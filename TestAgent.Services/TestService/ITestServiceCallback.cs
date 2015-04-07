using System.ServiceModel;
using TestAgent.Core;

namespace TestAgent.Services.TestService
{
    public interface ITestServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnTestOutputChanged(string output);
        [OperationContract(IsOneWay = true)]
        void OnTestFinished(TestResult result, string exceptionMessage, string exceptionStackTrace);
        [OperationContract(IsOneWay = true)]
        void OnTestRunFinished(TestResult result, string exceptionMessage, string exceptionStackTrace);
        [OperationContract(IsOneWay = true)]
        void OnTestRunStarted(string testFilename, int numberOfSelectedTests);
        [OperationContract(IsOneWay = true)]
        void OnTestStarted(string testName);
        [OperationContract(IsOneWay = true)]
        void OnUnhandledExceptionInTest(string exceptionMessage, string exceptionStackTrace);
    }
}