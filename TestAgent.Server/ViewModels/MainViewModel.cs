using System;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using TestAgent.Server.Properties;
using TestAgent.Services.FileService;
using TestAgent.Services.TestService;

namespace TestAgent.Server
{
    public class MainViewModel : Screen
    {
        private FileServiceHost _fileService;
        private TestServiceHost _testService;
        private string _uploadsDirectory;
        private string _fileServiceStatus;
        private string _testServiceStatus;
        private string _agentStatus;

        public MainViewModel()
        {
            UploadsDirectory = Settings.Default.UploadsDirectory;

            InitializeServices();

            StartMonitoringStates();
        }

        private void StartMonitoringStates()
        {
            // Launch the thread that monitors the services
            var monitorThread = new Thread(() =>
            {
                while (true)
                {
                    FileServiceStatus = _fileService.State == CommunicationState.Opened ? "Online" : "Offline";
                    TestServiceStatus = _testService.State == CommunicationState.Opened ? "Online" : "Offline";
                    Thread.Sleep(1000);
                }
            });
            monitorThread.IsBackground = true;
            monitorThread.Name = "Service Monitor Thread";
            monitorThread.Start();
        }

        private void InitializeServices()
        {
            if (_fileService != null && _fileService.State == CommunicationState.Opened)
            {
                _fileService.CloseService();
            }

            if (_testService != null && _testService.State == CommunicationState.Opened)
            {
                _testService.CloseService();
            }

            // Host the services
            _fileService = new FileServiceHost(9123, UploadsDirectory);
            _fileService.LaunchService();

            _testService = new TestServiceHost(9123, UploadsDirectory);
            _testService.LaunchService();
        }
        
        public string UploadsDirectory
        {
            get { return _uploadsDirectory; }
            set
            {
                if (value == _uploadsDirectory) return;
                _uploadsDirectory = value;
                Settings.Default.UploadsDirectory = value;
                Settings.Default.Save();
                NotifyOfPropertyChange(() => UploadsDirectory);

                // Reinitialize services
                InitializeServices();
            }
        }

        public string MachineName { get { return Environment.MachineName; } }

        public string FileServiceStatus
        {
            get { return _fileServiceStatus; }
            set
            {
                if (value == _fileServiceStatus) return;
                _fileServiceStatus = value;
                NotifyOfPropertyChange(() => FileServiceStatus);
            }
        }

        public string TestServiceStatus
        {
            get { return _testServiceStatus; }
            set
            {
                if (value == _testServiceStatus) return;
                _testServiceStatus = value;
                NotifyOfPropertyChange(() => TestServiceStatus);
            }
        }

        public string AgentStatus
        {
            get { return _agentStatus; }
            set
            {
                if (value == _agentStatus) return;
                _agentStatus = value;
                NotifyOfPropertyChange(() => AgentStatus);
            }
        }

        public void Hide()
        {
            Application.Current.MainWindow.Hide();
        }

        public void ShowMainWindow()
        {
            Application.Current.MainWindow.Show();
        }

        public void Exit()
        {
            Application.Current.MainWindow.Close();
        }

        public void SelectUploadsDirectory()
        {
            var uploadDirectoryPicker = new CommonOpenFileDialog
            {
                Title = "Chooose your uploads directory",
                IsFolderPicker = true,
                EnsurePathExists = true,
                Multiselect = false
            };

            if (uploadDirectoryPicker.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok)
            {
                UploadsDirectory = uploadDirectoryPicker.FileName;
            }
        }
    }
}
