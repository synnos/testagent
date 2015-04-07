namespace TestAgent.Core
{
    /// <summary>
    /// Holds the information about the results of a test run
    /// </summary>
    public struct TestSummary
    {
        /// <summary>
        /// The overall test run result
        /// </summary>
        public TestResult Result { get; set; }
        /// <summary>
        /// The exception message (if any)
        /// </summary>
        public string ExceptionMessage { get; set; }
        /// <summary>
        /// The exception call stack (if any)
        /// </summary>
        public string ExceptionCallStack { get; set; }
    }
}