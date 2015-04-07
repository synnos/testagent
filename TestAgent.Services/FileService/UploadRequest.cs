using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace TestAgent.Services.FileService
{
    [DataContract]
    [MessageContract]
    public class UploadRequest
    {
        [DataMember]
        [MessageHeader(MustUnderstand = true)]
        public string FileName { get; set; }

        [DataMember]
        [MessageBodyMember(Order = 1)]
        public Stream Stream { get; set; }
    }
}