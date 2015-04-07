using System.Runtime.Serialization;
using System.ServiceModel;

namespace TestAgent.Services.FileService
{
    [DataContract]
    [MessageContract]
    public class UploadAcknowledgement
    {
        [DataMember]
        [MessageBodyMember(Order = 1)]
        public bool UploadResult { get; set; }

        [DataMember]
        [MessageBodyMember(Order = 2)]
        public string Token { get; set; }
    }
}