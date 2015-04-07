using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Core;

namespace TestAgent.Core
{
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Convert from NUnit result type to ours
        /// </summary>
        /// <param name="state">The NUnit result state</param>
        /// <returns>The equivalent TestResult</returns>
        internal static TestResult ToTestResult(this ResultState state)
        {
            switch (state)
            {
                case ResultState.Cancelled:
                    return TestResult.Cancelled;

                case ResultState.Inconclusive:
                    return TestResult.Inconclusive;

                case ResultState.Success:
                    return TestResult.Passed;

                default:
                    return TestResult.Failed;
            }
        }
    }
}
