namespace TestAgent.Core
{
    public interface ITestDefinition
    {
        string Fullname { get; set; }
        string Name { get; set; }
        bool IsSelected { get; set; }
    }
}