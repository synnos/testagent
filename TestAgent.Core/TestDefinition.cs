namespace TestAgent.Core
{
    public class TestDefinition : ITestDefinition
    {
        public string Fullname { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}