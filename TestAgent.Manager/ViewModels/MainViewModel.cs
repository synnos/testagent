using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Caliburn.Micro;
using TestAgent.Services;

namespace TestAgent.Manager
{
    class MainViewModel : Screen, ITestAgentManager
    {
        private readonly List<TestAgentViewModel> _agents;
        private readonly IWindowManager _windowManager;
        private string _agentsFileLocation;
        private string _settingsFolder;

        public MainViewModel()
        {
            DisplayName = "Test Agent Manager";
            _agents = new List<TestAgentViewModel>();

            _windowManager = new WindowManager();

            _settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestAgentManager");

            _agentsFileLocation = Path.Combine(_settingsFolder, "agents.cfg");

            LoadSavedAgents();
        }

        private void LoadSavedAgents()
        {
            if (File.Exists(_agentsFileLocation))
            {
                var agents = TestAgentClient.ImportMultipleFromXml(_agentsFileLocation);
                foreach (var testAgentClient in agents)
                {
                    testAgentClient.Connect();
                    _agents.Add(new TestAgentViewModel(testAgentClient, this));
                }
                NotifyOfPropertyChange(() => TestAgentNames);
                NotifyOfPropertyChange(() => Agents);
            }
        }

        private void SaveAgents()
        {
            if (!Directory.Exists(_settingsFolder))
            {
                Directory.CreateDirectory(_settingsFolder);
            }

            TestAgentClient.ExportMultipleToXml(_agents.Select(a => a.Client), _agentsFileLocation);
        }

        public void AddAgent()
        {
            var addAgent = new AddAgentViewModel(_windowManager);
            if (_windowManager.ShowDialog(addAgent) == true)
            {
                var client = new TestAgentClient(addAgent.Hostname, addAgent.Port);
                client.Connect();
                _agents.Add(new TestAgentViewModel(client, this));

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

        public void OnClosing()
        {
            SaveAgents();
        }

        public void Remove(TestAgentClient client)
        {
            var agent = _agents.FirstOrDefault(a => a.Client == client);
            if (agent != null)
            {
                _agents.Remove(agent);
                NotifyOfPropertyChange(() => TestAgentNames);
                NotifyOfPropertyChange(() => Agents);
            }
        }
    }
}
