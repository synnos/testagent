using System;

namespace TestAgent.Core
{
    /// <summary>
    /// EventArgs that hold TestRunInformation
    /// </summary>
    public class TestRunInformationEventArgs : EventArgs
    {
        public TestRunInformationEventArgs(TestRunInformation information)
        {
            Information = information;
        }

        public TestRunInformation Information { get; private set; }
    }
}