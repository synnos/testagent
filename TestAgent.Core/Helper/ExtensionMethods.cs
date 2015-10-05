using System.Xml;
using NUnit.Core;

namespace TestAgent.Core
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Convert from NUnit result type to ours
        /// </summary>
        /// <param name="state">The NUnit result state</param>
        /// <returns>The equivalent TestResult</returns>
        public static TestResult ToTestResult(this ResultState state)
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

        /// <summary>
        /// Writes a child node in the XmlWriter object
        /// </summary>
        /// <param name="writer">The XmlWriter to use</param>
        /// <param name="elementName">The name of the XML element</param>
        /// <param name="elementValue">The value of the element</param>
        public static void WriteXmlChild(this XmlWriter writer, string elementName, object elementValue)
        {
            writer.WriteStartElement(elementName);
            writer.WriteString(elementValue.ToString());
            writer.WriteEndElement();
        }
    }
}
