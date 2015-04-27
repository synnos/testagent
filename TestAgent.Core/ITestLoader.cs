namespace TestAgent.Core
{
    public interface ITestLoader
    {
        TestCollection[] LoadTests(string filename);
    }
}