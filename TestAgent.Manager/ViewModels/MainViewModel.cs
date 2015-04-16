using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using TestAgent.Services;

namespace TestAgent.Manager
{
    class MainViewModel : Screen
    {
        private readonly List<TestAgentViewModel> _clients; 

        public MainViewModel()
        {
            DisplayName = "Test Agent Manager";
            _clients = new List<TestAgentViewModel>();

            LoadSavedAgents();
        }

        private void LoadSavedAgents()
        {
            // TODO: Load agents from previous sessions
        }

        public void AddAgent(string hostname, int port)
        {
            var client = new TestAgentClient(hostname, port);
            client.Connect();
            _clients.Add(new TestAgentViewModel(client));

            NotifyOfPropertyChange(() => TestAgentNames);
            NotifyOfPropertyChange(() => Clients);
        }

        public string[] TestAgentNames
        {
            get { return _clients.Select(c => c.Hostname).ToArray(); }
        }

        public TestAgentViewModel[] Clients
        {
            get { return _clients.ToArray(); }
        }
    }
}
