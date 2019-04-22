using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Newtonsoft.Json;
using PactNet.Configuration.Json.Converters;
using PactNet.ProtobufReflection;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class ProviderServiceResponse : IHttpMessage
    {
        private bool _bodyWasSet;
        private dynamic _body;

        [JsonProperty(PropertyName = "status", NullValueHandling = NullValueHandling.Ignore)]
        public int Status { get; set; }
        
        // Encoding object with type and description, generate this dynamically if the body is an IMessage
        [JsonProperty(PropertyName = "encoding", NullValueHandling = NullValueHandling.Ignore)]
        public object Encoding {
            get
            {
                if (_bodyWasSet &&  _body is IMessage)
                {
                    var messageType = (IMessage) _body;
                    var descriptor = messageType.Descriptor.File;
                    
                    return new
                    {
                        Type = "protobuf",
                        Description = new
                        {
                            MessageName = messageType.Descriptor.Name,
                            FileDescriptorSet = FileDescriptorSetBytes.Get(descriptor)
                        }
                    };
                }

                return null;
            } 
        }

        [JsonProperty(PropertyName = "headers", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(PreserveCasingDictionaryConverter))]
        public IDictionary<string, object> Headers { get; set; }

        [JsonProperty(PropertyName = "body")]
        public dynamic Body
        {
            get { return _body; }
            set
            {
                _bodyWasSet = true;
                _body = value;
            }
        }

        // A not so well known feature in JSON.Net to do conditional serialization at runtime
        public bool ShouldSerializeBody()
        {
            return _bodyWasSet;
        }
    }
}