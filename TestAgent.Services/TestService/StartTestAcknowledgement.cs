using System.Runtime.Serialization;

namespace TestAgent.Services.TestService
{
    [DataContract]
    public class StartTestAcknowledgement
    {
        [DataMember]
        public bool Started { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}