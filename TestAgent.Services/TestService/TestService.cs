using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using TestAgent.Core;
using TestAgent.Services.FileService;

namespace TestAgent.Services.TestService
{
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        UseSynchronizationContext = false,
        AddressFilterMode = AddressFilterMode.Any)]
    public class TestService : ITestService
    {
        private readonly Dictionary<string, string> _clientSessions;
        private readonly Dictionary<string, ITestServiceCallback> _sessionCallbacks;
        private readonly Dictionary<TestType, ITestRunner> _testRunners; 
        private string _currentSession;

        public TestService()
        {
            _clientSessions = new Dictionary<string, string>();
            _sessionCallbacks = new Dictionary<string, ITestServiceCallback>();
            _testRunners = new Dictionary<TestType, ITestRunner>();

            // Add the default test runners. This section could be done using MEF in the future
            var nunitTestRunner = new NUnitTestRunner();
            nunitTestRunner.TestFinished += nunitTestRunner_TestFinished;
            nunitTestRunner.TestOutput += nunitTestRunner_testOutput;
            nunitTestRunner.TestRunFinished += nunitTestRunner_TestRunFinished;
            nunitTestRunner.TestRunStarted += nunitTestRunner_TestRunStarted;
            nunitTestRunner.TestStarted += nunitTestRunner_TestStarted;
            nunitTestRunner.UnhandledExceptionInTest += nunitTestRunner_UnhandledExceptionInTest;
            _testRunners.Add(TestType.NUnit, nunitTestRunner);
        }

        void nunitTestRunner_UnhandledExceptionInTest(object sender, TestSummary e)
        {
            if (_sessionCallbacks.ContainsKey(_currentSession))
            {
                var notificationCallback = _sessionCallbacks[_currentSession];
                if (notificationCallback != null)
                {
                    notificationCallback.OnUnhandledExceptionInTest(e.ExceptionMessage, e.ExceptionCallStack);
                }
            }
        }

        void nunitTestRunner_TestStarted(object sender, TestInformation e)
        {
            if (_sessionCallbacks.ContainsKey(_currentSession))
            {
                var notificationCallback = _sessionCallbacks[_currentSession];
                if (notificationCallback != null)
                {
                    notificationCallback.OnTestStarted(e.TestName);
                }
            }
        }

        void nunitTestRunner_TestRunStarted(object sender, TestRunInformation e)
        {
            if (_sessionCallbacks.ContainsKey(_currentSession))
            {
                var notificationCallback = _sessionCallbacks[_currentSession];
                if (notificationCallback != null)
                {
                    notificationCallback.OnTestRunStarted(e.Filename, e.NumberOfTests);
                }
            }
        }

        void nunitTestRunner_TestRunFinished(object sender, TestSummary e)
        {
            if (_sessionCallbacks.ContainsKey(_currentSession))
            {
                var notificationCallback = _sessionCallbacks[_currentSession];
                if (notificationCallback != null)
                {
                    notificationCallback.OnTestRunFinished(e.Result, e.ExceptionMessage, e.ExceptionCallStack);
                }
            }
        }

        void nunitTestRunner_TestFinished(object sender, TestSummary e)
        {
            if (_sessionCallbacks.ContainsKey(_currentSession))
            {
                var notificationCallback = _sessionCallbacks[_currentSession];
                if (notificationCallback != null)
                {
                    notificationCallback.OnTestFinished(e.Result, e.ExceptionMessage, e.ExceptionCallStack);
                }
            }
        }

        private void nunitTestRunner_testOutput(object sender, string s)
        {
            if (_sessionCallbacks.ContainsKey(_currentSession))
            {
                var notificationCallback = _sessionCallbacks[_currentSession];
                if (notificationCallback != null)
                {
                    notificationCallback.OnTestOutputChanged(s);
                }
            }
        }

        public string UploadDirectory { get; set; }

        public void Register(string clientId)
        {
            var clientSession = GetClientSessionId();
            if (_clientSessions.ContainsKey(clientId) && _clientSessions[clientId] != clientSession)
            {
                DisconnectClient(_clientSessions[clientId]);
                _clientSessions[clientId] = clientSession;
            }
            else if (!_clientSessions.ContainsKey(clientId))
            {
                _clientSessions.Add(clientId, clientSession);
            }

            if (!_sessionCallbacks.ContainsKey(clientSession))
            {
                _sessionCallbacks.Add(clientSession, OperationContext.Current.GetCallbackChannel<ITestServiceCallback>());
            }
        }

        private void DisconnectClient(string clientSession)
        {
            // TODO
        }

        public StartTestAcknowledgement StartTest(string fileToken, string testFilename, TestType testType, string[] testNames)
        {
            var runner = _testRunners[testType];
            // Check if the test runner is busy
            if (runner.State != TestRunnerState.Idle)
            {
                return new StartTestAcknowledgement()
                {
                    Started = false,
                    Message = "The test runner is busy!"
                };
            }

            // Check if the client has registered their session
            if (!_clientSessions.ContainsValue(GetClientSessionId()))
            {
                return new StartTestAcknowledgement()
                {
                    Started = false,
                    Message = "The client needs to register on the agent first!"
                };
            }

            // Check if the files determined by the token exist
            var fullDirectory = Path.Combine(UploadDirectory, fileToken);
            if (!Directory.Exists(fullDirectory))
            {
                return new StartTestAcknowledgement()
                {
                    Started = false,
                    Message = "Could not find the files needed for the test!"
                };
            }

            // Check if the file specified for the tests exists
            var testFile = Path.Combine(fullDirectory, testFilename);
            if (!File.Exists(testFile))
            {
                return new StartTestAcknowledgement()
                {
                    Started = false,
                    Message = "The test file could not be found!"
                };
            }

            _currentSession = GetClientSessionId();
            runner.CopyDependencies(fullDirectory);
            runner.BeginRunTest(testFile, testNames);
            return new StartTestAcknowledgement()
            {
                Started = true
            };
        }

        public bool AliveCheck()
        {
            return true;
        }
        
        private static string GetClientSessionId()
        {
            if (OperationContext.Current != null)
            {
                if (OperationContext.Current.Channel != null)
                {
                    return OperationContext.Current.SessionId;
                }
            }

            return string.Empty;
        }
    }
}