using System;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace PactNet.ProtobufReflection
{
    public static class FileDescriptorSetBytes
    {
        // Note: This is a hack around the fact that the Google-defined FileDescriptorSet is internal to the protobuf
        // implementation (and should probably remain so) - here we construct something that's binary identical to
        // FileDescriptorSet when serialized.
        public static ByteString Get(FileDescriptor descriptor)
        {
            var dependencies = descriptor.Dependencies.Select(x => x.SerializedData);
            var selfProto = descriptor.SerializedData;

            var fds = new FileDescriptorSet();
            fds.EncodedFds.AddRange(dependencies);
            fds.EncodedFds.Add(selfProto);

            return fds.ToByteString();
        }
    }
}