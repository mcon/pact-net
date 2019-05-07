using System;
using PactNet;
using PactNet.Models;

namespace ProtobufPactMockWrapper
{
    public class CustomSerializationHostConfig
    {
        public string Arguments { get; }
        public string Script { get; }
        
        public CustomSerializationHostConfig(int port,
            string pactDir, string host, string sslCert, string sslKey, Uri rubyCoreUrl, bool verification = false)
        {
            // TODO: Add SSL functionality
            Arguments = $"--pact-dir {pactDir} --host {host} --ruby-core-url {rubyCoreUrl} --port {port}" +
                (verification ? " --verification" : string.Empty);
            
            
            // TODO: Once development has finished - point to correct directory
            Script = @"/home/matt/go/src/github.com/mcon/pact-serialization-proxy/proxy-server";
        }
    }
}