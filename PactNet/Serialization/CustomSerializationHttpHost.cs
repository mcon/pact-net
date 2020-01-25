using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using PactNet;
using PactNet.Mocks.MockHttpService.Host;
using PactNet.Models;

namespace ProtobufPactMockWrapper
{
    internal class CustomSerializationHttpHost : IHttpHost
    {
        private Uri _rubyCoreUri;
        private readonly Uri _baseUri;
        private readonly string _consumerName;
        private readonly string _providerName;
        private readonly PactConfig _config;
        private readonly IPAddress _host;
        private readonly string _sslCert;
        private readonly string _sslKey;
        private RubyHttpHost _baseRubyHost;
        private Thread _wrapperHost;

        public CustomSerializationHttpHost(Uri baseUri, Uri rubyUri,
            string consumerName, string providerName, PactConfig config, IPAddress host = IPAddress.Loopback,
            string sslCert = null, string sslKey = null)
        {
            _baseUri = baseUri;
            _consumerName = consumerName;
            _sslCert = sslCert;
            _sslKey = sslKey;
            _providerName = providerName;
            _config = config;
            _host = host;
            _rubyCoreUri = rubyUri;
        }
        public void Start()
        {
            _baseRubyHost = new RubyHttpHost(_rubyCoreUri, _consumerName, _providerName, _config, _host, _sslCert, _sslKey);
            
            _baseRubyHost.Start();
            StartWrapper();
        }

        private void StartWrapper()
        {
            var config = new CustomSerializationHostConfig(_baseUri.Port, _config.PactDir, _baseUri.Host, _sslCert, _sslKey, _rubyCoreUri);
            var startInfo = new ProcessStartInfo
            {
#if USE_NET4X
                WindowStyle = ProcessWindowStyle.Hidden,
#endif
                FileName = config.Script,
                Arguments = config.Arguments,
                UseShellExecute = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            
            var process = new Process{StartInfo = startInfo};
            process.OutputDataReceived += WriteLineToOutput;
            process.ErrorDataReceived += WriteLineToOutput;
            
            var success = process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            if (!success)
            {
                throw new PactFailureException("Could not start the Pact Core Host");
            }

            // TODO: Actually perform healthcheck - sleep for now
            Thread.Sleep(3000);
        }

        public void Stop()
        {
            _baseRubyHost.Stop();
            StopWrapper();
            
        }

        private void StopWrapper()
        {
            // TODO: Should kill the serialization-proxy at this point
            //_wrapperHost.Abort();
        }
        
        private void WriteLineToOutput(object sender, DataReceivedEventArgs eventArgs)
        {
            if (eventArgs.Data != null)
            {
                WriteToOutputters(Regex.Replace(eventArgs.Data, @"\e\[(\d+;)*(\d+)?[ABCDHJKfmsu]", ""));
            }
        }

        private void WriteToOutputters(string line)
        {
            if (_config.Outputters != null && _config.Outputters.Any())
            {
                foreach (var output in _config.Outputters)
                {
                    output.WriteLine(line);
                }
            }
        }
    }
}