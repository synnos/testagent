using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using TestAgent.Core;
using TestAgent.Services;
using TestAgent.Services.FileService;

namespace TestAgent.Manager
{
    class MainViewModel : Screen, ITestAgentManager
    {
        private readonly List<TestAgentViewModel> _agents;
        private readonly IWindowManager _windowManager;
        private readonly string _agentsFileLocation;
        private readonly string _settingsFolder;
        private TestDefinitionViewModel[] _currentTests;

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
                    testAgentClient.TestOutputReceived += client_TestOutputReceived;
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

            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Owner = GetView();

            if (_windowManager.ShowDialog(addAgent, null, settings) == true)
            {
                var client = new TestAgentClient(addAgent.Hostname, addAgent.Port);
                client.Connect();
                _agents.Add(new TestAgentViewModel(client, this));
                client.TestOutputReceived += client_TestOutputReceived;

                NotifyOfPropertyChange(() => TestAgentNames);
                NotifyOfPropertyChange(() => Agents);
            }
        }

        void client_TestOutputReceived(object sender, string e)
        {
            // TODO
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
                client.TestOutputReceived -= client_TestOutputReceived;
                NotifyOfPropertyChange(() => TestAgentNames);
                NotifyOfPropertyChange(() => Agents);
            }
        }

        public void OpenTests()
        {
            var openTests = new OpenViewModel();

            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Owner = GetView();

            if (_windowManager.ShowDialog(openTests, null, settings) == true)
            {
                TestDependencies = openTests.Dependencies;
                TestFile = openTests.TestFile;

                // Since we only support NUnit for now we can just create an NUnit loader
                // This will definitely require some better design once we add more test types
                var testLoader = new NUnitTestLoader();

                CurrentTests = testLoader.LoadTests(TestFile).Select(c => new TestCollectionViewModel(c)).ToArray();
            }
        }

        public string[] TestDependencies { get; set; }

        public string TestFile { get; set; }

        public TestDefinitionViewModel[] CurrentTests
        {
            get { return _currentTests; }
            private set
            {
                if (Equals(value, _currentTests)) return;
                _currentTests = value;
                NotifyOfPropertyChange(() => CurrentTests);
            }
        }

        public void RunSelectedTests()
        {
            // Check that we have some opened tests
            if (CurrentTests == null || CurrentTests.Length == 0)
            {
                var warning =
                    new WarningViewModel("You must first open some tests in order to run them!",
                        "No opened tests");

                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.Owner = GetView();

                _windowManager.ShowDialog(warning, null, settings);
                return;
            }

            // Check that we have some selected tests
            if (CurrentTests.SelectMany(t=>((TestCollectionViewModel)t).AllSelectedTests).ToArray().Length == 0)
            {
                var warning =
                    new WarningViewModel("You must first select some tests in order to run them!",
                        "No selected tests");

                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.Owner = GetView();

                _windowManager.ShowDialog(warning, null, settings);
                return;
            }

            // Check that we have at least one agent connected
            if (_agents.All(a => !a.Client.IsConnected))
            {
                var warning =
                    new WarningViewModel("You need to be connected to at least one agent in order to run the tests!",
                        "No available agents");

                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.Owner = GetView();

                _windowManager.ShowDialog(warning, null, settings);
                return;
            }

            // Zip all files
            string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "tmp.zip");

            var files = TestDependencies == null ? new List<string>() : TestDependencies.ToList();

            if (!files.Contains(TestFile))
            {
                files.Add(TestFile);
            }

            FilePackager.CompressFiles(tempFile, files.ToArray());

            using (var stream = new FileStream(tempFile, FileMode.Open))
            {
                var uploadResult = _agents[0].Client.FileService.Client.UploadFile(new UploadRequest()
                {
                    FileName = "TempFile.zip",
                    Stream = stream
                });

                if (!uploadResult.UploadResult)
                {
                    var warning =
                    new WarningViewModel("Could not upload the files to the test agent!",
                        "File upload failed");

                    dynamic settings = new ExpandoObject();
                    settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    settings.Owner = GetView();

                    _windowManager.ShowDialog(warning, null, settings);
                    return;
                }

                var names = CurrentTests.SelectMany(c => ((TestCollectionViewModel) c).AllSelectedTests).ToArray();

                _agents[0].Client.TestService.Client.Register(Environment.MachineName);

                string testFile = Path.GetFileName(TestFile);
                var t = new Thread(() =>
                _agents[0].Client.TestService.Client.StartTest(uploadResult.Token, testFile, TestType.NUnit, names));
                t.Start();
            }

            File.Delete(tempFile);
            
            // TODO: Disable Open/Run/Remove agent and enable Cancel
        }

        public void CancelTestRun()
        {
            // TODO
        }
    }
}
