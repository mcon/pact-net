using System;
using System.Threading;
using PactNet;
using PactNet.Mocks.MockHttpService.Host;
using PactNet.Models;

namespace ProtobufPactMockWrapper
{
    internal class CustomSerializationHttpHost : IHttpHost
    {
        private Uri _rubyCoreUri;
        private readonly string _consumerName;
        private readonly string _providerName;
        private readonly PactConfig _config;
        private readonly IPAddress _host;
        private readonly string _sslCert;
        private readonly string _sslKey;
        private RubyHttpHost _baseRubyHost;
        private Thread _wrapperHost;

        public CustomSerializationHttpHost(Uri baseUri,
            string consumerName, string providerName, PactConfig config, IPAddress host = IPAddress.Loopback,
            string sslCert = null, string sslKey = null)
        {
            _consumerName = consumerName;
            _sslCert = sslCert;
            _sslKey = sslKey;
            _providerName = providerName;
            _config = config;
            _host = host;
            
            // TODO: Pick a random port for Ruby HTTP host to run on - for now fix it
            _rubyCoreUri = new UriBuilder(baseUri){Port = 8888}.Uri;
        }
        public void Start()
        {
            _baseRubyHost = new RubyHttpHost(_rubyCoreUri, _consumerName, _providerName, _config, _host, _sslCert, _sslKey);
            
            _baseRubyHost.Start();
            StartWrapper();
        }

        private void StartWrapper()
        {
            
            // TODO: Start go mock server proxy
            
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
            // TODO: Should treat the web server as a process and not a thread.
            //_wrapperHost.Abort();
        }
    }
}