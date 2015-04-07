using System.ServiceModel;
using TestAgent.Core;

namespace TestAgent.Services.TestService
{
    [ServiceKnownType(typeof(TestType))]
    [ServiceKnownType(typeof(TestResult))]
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof (ITestServiceCallback))]
    public interface ITestService
    {
        [OperationContract]
        void Register(string clientId);
        [OperationContract]
        StartTestAcknowledgement StartTest(string fileToken, string testFilename, TestType testType, string[] testNames);
    }
}
