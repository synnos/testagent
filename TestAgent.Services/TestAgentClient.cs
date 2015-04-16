using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestAgent.Services.FileService;
using TestAgent.Services.TestService;

namespace TestAgent.Services
{
    public class TestAgentClient
    {
        private FileServiceClientHost _fileClient;
        private TestServiceClientHost _testClient;
        private bool _isConnected;
        private bool _isRunning;

        public event EventHandler IsConnectedChanged;

        protected virtual void OnIsConnectedChanged()
        {
            EventHandler handler = IsConnectedChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public TestAgentClient(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
        }

        public void Connect()
        {
            if (IsConnected)
            {
                // We are already connected
                return;
            }

            _fileClient = new FileServiceClientHost();
            _fileClient.Connect(Hostname, Port);

            _testClient = new TestServiceClientHost();
            _testClient.Connect(Hostname, Port);

            _isRunning = true;

            StartMonitoringConnectionStatus();
        }

        private void StartMonitoringConnectionStatus()
        {
            var connectionStatusMonitor = new Thread(() =>
            {
                while (_isRunning)
                {
                    IsConnected = _fileClient.State == ConnectionState.Online &&
                                  _testClient.State == ConnectionState.Online;

                    Thread.Sleep(500);
                }
            });
            connectionStatusMonitor.IsBackground = true;
            connectionStatusMonitor.Name = string.Format("{0}_{1}_Monitor", Hostname, Port);
            connectionStatusMonitor.Start();
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            private set
            {
                if (value == _isConnected)
                {
                    return;
                }

                _isConnected = value;

                OnIsConnectedChanged();
            }
        }

        public string Hostname { get; private set; }

        public int Port { get; private set; }
    }
}
