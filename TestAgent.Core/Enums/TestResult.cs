using System.Runtime.Serialization;

namespace TestAgent.Core
{
    /// <summary>
    /// The possible test run result states
    /// </summary>
    [DataContract]
    public enum TestResult
    {
        [EnumMember]
        Passed,
        [EnumMember]
        Failed,
        [EnumMember]
        Inconclusive,
        [EnumMember]
        Cancelled,
        [EnumMember]
        Running
    }
}