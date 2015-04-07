using System;

namespace TestAgent.Core
{
    /// <summary>
    /// Should be implemented by each unique test runner
    /// </summary>
    public interface ITestRunner
    {
        /// <summary>
        /// Executes the tests specified that can be found inside the file specified
        /// </summary>
        /// <param name="filename">The file that contains the tests</param>
        /// <param name="testNames">The names of the tests to execute</param>
        void BeginRunTest(string filename, string[] testNames);
        /// <summary>
        /// Cancels the test execution
        /// </summary>
        void CancelRunTest();
        /// <summary>
        /// Copies any required files needed by the test runner.
        /// For example: NUnit needs nunit.core.dll, nunit.framework.dll and nunit.core.interfaces.dll
        /// </summary>
        /// <param name="directory">The directory in which to copy the files</param>
        void CopyDependencies(string directory);
        /// <summary>
        /// Gets raised when a test run starts
        /// </summary>
        event EventHandler<TestRunInformation> TestRunStarted;
        /// <summary>
        /// Gets raised when a test run finishes
        /// </summary>
        event EventHandler<TestSummary> TestRunFinished;
        /// <summary>
        /// Gets raised when a single test starts
        /// </summary>
        event EventHandler<TestInformation> TestStarted;
        /// <summary>
        /// Gets raised when a single test finishes
        /// </summary>
        event EventHandler<TestSummary> TestFinished;
        /// <summary>
        /// Gets raised when there is an unhandled exception in a test
        /// </summary>
        event EventHandler<TestSummary> UnhandledExceptionInTest;
        /// <summary>
        /// Gets raised when there is any new output from the test being executed
        /// </summary>
        event EventHandler<string> TestOutput;
        /// <summary>
        /// Gets raised whenever the state of the test runner changes
        /// </summary>
        event EventHandler StateChanged;
        /// <summary>
        /// Gets the state of the test runner [Default = Idle]
        /// </summary>
        TestRunnerState State { get; }
    }
}
