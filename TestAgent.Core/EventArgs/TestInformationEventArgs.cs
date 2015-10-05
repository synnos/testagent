using System;

namespace TestAgent.Core
{
    /// <summary>
    /// EventArgs that holds an instance of TestInformation
    /// </summary>
    public class TestInformationEventArgs : EventArgs
    {
        public TestInformationEventArgs(TestInformation information)
        {
            Information = information;
        }

        public TestInformation Information { get; private set; }
    }
}