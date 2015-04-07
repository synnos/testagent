using System;
using System.IO;
using System.Threading;
using NUnit.Core;
using NUnit.Core.Filters;

namespace TestAgent.Core
{
    public class NUnitTestRunner : EventListener, ITestRunner
    {
        private TestRunnerState _state;
        private readonly RemoteTestRunner _testRunner;

        public NUnitTestRunner()
        {
            State = TestRunnerState.Idle;
            _testRunner = new RemoteTestRunner();
        }

        public TestRunnerState State
        {
            get { return _state; }
            private set
            {
                if (value == _state) return;
                _state = value;
                OnStateChanged();
            }
        }

        public void BeginRunTest(string filename, string[] testNames)
        {
            State = TestRunnerState.Running;

            var nameFilter = new SimpleNameFilter();
            foreach (var testName in testNames)
            {
                nameFilter.Add(testName);
            }

            CoreExtensions.Host.InitializeService();
            var testPackage = new TestPackage(filename);
            _testRunner.Load(testPackage);

            _testRunner.BeginRun(this, nameFilter);

            var stateCheckerThread = new Thread(() =>
            {
                while (_testRunner.Running)
                {
                    Thread.Sleep(100);
                }
                State = TestRunnerState.Idle;
            });
            stateCheckerThread.IsBackground = true;
            stateCheckerThread.Name = "stateCheckerThread";
            stateCheckerThread.Start();
        }

        public void CancelRunTest()
        {
            if (_testRunner.Running)
            {
                _testRunner.CancelRun();
            }
        }

        public void CopyDependencies(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentException("Output directory cannot be null or empty", "directory");
            }

            File.Copy("nunit.core.dll", Path.Combine(directory, "nunit.core.dll"));
            File.Copy("nunit.core.interfaces.dll", Path.Combine(directory, "nunit.core.interfaces.dll"));
            File.Copy("nunit.framework.dll", Path.Combine(directory, "nunit.framework.dll"));
        }

        public event EventHandler StateChanged;

        protected virtual void OnStateChanged()
        {
            EventHandler handler = StateChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<TestRunInformation> TestRunStarted;

        protected virtual void OnTestRunStarted(TestRunInformation e)
        {
            EventHandler<TestRunInformation> handler = TestRunStarted;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<TestSummary> TestRunFinished;

        protected virtual void OnTestRunFinished(TestSummary e)
        {
            EventHandler<TestSummary> handler = TestRunFinished;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<TestInformation> TestStarted;

        protected virtual void OnTestStarted(TestInformation e)
        {
            EventHandler<TestInformation> handler = TestStarted;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<TestSummary> TestFinished;

        protected virtual void OnTestFinished(TestSummary e)
        {
            EventHandler<TestSummary> handler = TestFinished;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<TestSummary> UnhandledExceptionInTest;

        protected virtual void OnUnhandledExceptionInTest(TestSummary e)
        {
            EventHandler<TestSummary> handler = UnhandledExceptionInTest;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<string> TestOutput;

        protected virtual void OnTestOutput(string e)
        {
            EventHandler<string> handler = TestOutput;
            if (handler != null) handler(this, e);
        }

        public void RunStarted(string name, int testCount)
        {
            OnTestRunStarted(new TestRunInformation { Filename = name, NumberOfTests = testCount });
        }

        public void RunFinished(NUnit.Core.TestResult result)
        {
            OnTestRunFinished(new TestSummary { Result = result.ResultState.ToTestResult() });
        }

        public void RunFinished(Exception exception)
        {
            OnTestRunFinished(new TestSummary { Result = TestResult.Failed, ExceptionMessage = exception.Message, ExceptionCallStack = exception.StackTrace });
        }

        void EventListener.TestStarted(TestName testName)
        {
            OnTestStarted(new TestInformation { TestName = testName.FullName });
        }

        void EventListener.TestFinished(NUnit.Core.TestResult result)
        {
            OnTestFinished(new TestSummary { Result = result.ResultState.ToTestResult() });
        }

        public void SuiteStarted(TestName testName)
        {
        }

        public void SuiteFinished(NUnit.Core.TestResult result)
        {
        }

        public void UnhandledException(Exception exception)
        {
            OnUnhandledExceptionInTest(new TestSummary { ExceptionCallStack = exception.StackTrace, ExceptionMessage = exception.Message, Result = TestResult.Failed });
        }

        void EventListener.TestOutput(TestOutput testOutput)
        {
            OnTestOutput(testOutput.Text);
        }
    }
}