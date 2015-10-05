using System;

namespace TestAgent.Core
{
    /// <summary>
    /// EventArgs that hold an instance of TestSummary
    /// </summary>
    public class TestSummaryEventArgs : EventArgs
    {
        public TestSummaryEventArgs(TestSummary summary)
        {
            Summary = summary;
        }

        public TestSummary Summary { get; private set; }
    }
}