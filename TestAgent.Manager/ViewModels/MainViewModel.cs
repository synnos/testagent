using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using TestAgent.Services;

namespace TestAgent.Manager
{
    class MainViewModel : Screen
    {
        private readonly List<TestAgentViewModel> _agents;
        private readonly IWindowManager _windowManager;

        public MainViewModel()
        {
            DisplayName = "Test Agent Manager";
            _agents = new List<TestAgentViewModel>();

            _windowManager = new WindowManager();

            LoadSavedAgents();
        }

        private void LoadSavedAgents()
        {
            // TODO: Load agents from previous sessions
        }

        public void AddAgent()
        {
            var addAgent = new AddAgentViewModel(_windowManager);
            if (_windowManager.ShowDialog(addAgent) == true)
            {
                var client = new TestAgentClient(addAgent.Hostname, addAgent.Port);
                client.Connect();
                _agents.Add(new TestAgentViewModel(client));

                NotifyOfPropertyChange(() => TestAgentNames);
                NotifyOfPropertyChange(() => Agents);
            }
        }

        public string[] TestAgentNames
        {
            get { return _agents.Select(c => c.Hostname).ToArray(); }
        }

        public TestAgentViewModel[] Agents
        {
            get { return _agents.ToArray(); }
        }
    }
}
