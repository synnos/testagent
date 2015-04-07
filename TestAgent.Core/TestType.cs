using System.Runtime.Serialization;

namespace TestAgent.Core
{
    /// <summary>
    /// The possible/supported test types
    /// </summary>
    [DataContract]
    public enum TestType
    {
        [EnumMember]
        NUnit
    }
}