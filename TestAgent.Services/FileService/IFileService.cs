using System.ServiceModel;

namespace TestAgent.Services.FileService
{
    /// <summary>
    /// This service is responsible for receiving files from the clients to the test agent. 
    /// Once the file is received the client receives a "token" which can be used to call tests in the files received.
    /// </summary>
    [ServiceContract(Namespace = "http://testagent/filetransfer")]
    public interface IFileService
    {
        [OperationContract]
        UploadAcknowledgement UploadFile(UploadRequest request);
    }
}
