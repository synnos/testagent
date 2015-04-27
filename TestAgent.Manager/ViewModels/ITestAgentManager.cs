using TestAgent.Services;

namespace TestAgent.Manager
{
    public interface ITestAgentManager
    {
        void Remove(TestAgentClient client);
    }
}