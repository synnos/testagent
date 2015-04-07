namespace TestAgent.Core
{
    /// <summary>
    /// Holds information about a test run
    /// </summary>
    public struct TestRunInformation
    {
        /// <summary>
        /// The filename that contains the tests
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// The number of tests selected to be executed
        /// </summary>
        public int NumberOfTests { get; set; }
    }
}