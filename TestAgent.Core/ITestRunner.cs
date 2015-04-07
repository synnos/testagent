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
    }
}
