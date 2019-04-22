using System;
using System.Collections.Generic;
using PactNet;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Host;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Models;

namespace ProtobufPactMockWrapper
{
    public class CustomSerializationMockProvider : IMockProviderService
    {
        private AdminHttpClient _adminHttpClient;
        private IHttpHost _host;
        private string _providerState;
        private string _description;
        private ProviderServiceRequest _request;
        private ProviderServiceResponse _response;

        public Uri BaseUri { get; }
        
        public CustomSerializationMockProvider(
            int port, bool enableSsl, string consumerName, string providerName, PactConfig config, IPAddress ipAddress, 
            Newtonsoft.Json.JsonSerializerSettings jsonSerializerSettings, string sslCert, string sslKeys)
        {
            BaseUri = new Uri( $"{(enableSsl ? "https" : "http")}://localhost:{port}");
            _adminHttpClient = new AdminHttpClient(BaseUri, jsonSerializerSettings);
            _host = new CustomSerializationHttpHost(BaseUri, consumerName, providerName, config, ipAddress, sslCert, sslKeys);
        }
        public IMockProviderService Given(string providerState)
        {
            if (String.IsNullOrEmpty(providerState))
            {
                throw new ArgumentException("Please supply a non null or empty providerState");
            }

            _providerState = providerState;

            return this;
        }

        public IMockProviderService UponReceiving(string description)
        {
            if (String.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Please supply a non null or empty description");
            }

            _description = description;

            return this;
        }

        public IMockProviderService With(ProviderServiceRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException("Please supply a non null request");
            }

            if (request.Method == HttpVerb.NotSet)
            {
                throw new ArgumentException("Please supply a request Method");
            }

            if (!IsContentTypeSpecifiedForBody(request))
            {
                throw new ArgumentException("Please supply a Content-Type request header");
            }

            _request = request;

            return this;
        }

        public void WillRespondWith(ProviderServiceResponse response)
        {
            if (response == null)
            {
                throw new ArgumentException("Please supply a non null response");
            }

            if (response.Status <= 0)
            {
                throw new ArgumentException("Please supply a response Status");
            }

            if (!IsContentTypeSpecifiedForBody(response))
            {
                throw new ArgumentException("Please supply a Content-Type response header");
            }

            _response = response;

            RegisterInteraction();
        }
        
        private void RegisterInteraction()
        {
            // TODO MC: Actually just need a ProviderServiceInteractionFactory
            if (String.IsNullOrEmpty(_description))
            {
                throw new InvalidOperationException("description has not been set, please supply using the UponReceiving method.");
            }

            if (_request == null)
            {
                throw new InvalidOperationException("request has not been set, please supply using the With method.");
            }

            if (_response == null)
            {
                throw new InvalidOperationException("response has not been set, please supply using the WillRespondWith method.");
            }

            var interaction = new ProviderServiceInteraction
            {
                ProviderState = _providerState,
                Description = _description,
                Request = _request,
                Response = _response
            };

            _adminHttpClient.SendAdminHttpRequest(HttpVerb.Post, Constants.InteractionsPath, interaction);

            ClearTrasientState();
        }

        public void Start()
        {
            _host.Start();
        }

        public void Stop()
        {
            ClearAllState();
            StopRunningHost();
        }

        public void ClearInteractions()
        {
            if (_host != null)
            {
                _adminHttpClient.SendAdminHttpRequest(HttpVerb.Delete, $"{Constants.InteractionsPath}");
            }
        }

        public void VerifyInteractions()
        {
            _adminHttpClient.SendAdminHttpRequest(HttpVerb.Get, $"{Constants.InteractionsVerificationPath}");
        }

        public void SendAdminHttpRequest(HttpVerb method, string path)
        {
            _adminHttpClient.SendAdminHttpRequest(method, path);
        }
        
        private void StopRunningHost()
        {
            if (_host != null)
            {
                _host.Stop();
                _host = null;
            }
        }

        private void ClearAllState()
        {
            ClearTrasientState();
            ClearInteractions();
        }

        private void ClearTrasientState()
        {
            _request = null;
            _response = null;
            _providerState = null;
            _description = null;
        }
        
        private bool IsContentTypeSpecifiedForBody(IHttpMessage message)
        {
            //No content-type required if there is no body
            if (message.Body == null)
            {
                return true;
            }

            IDictionary<string, object> headers = null;
            if (message.Headers != null)
            {
                headers = new Dictionary<string, object>(message.Headers, StringComparer.OrdinalIgnoreCase);
            }

            return headers != null && headers.ContainsKey("Content-Type");
        }
    }
}