using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TestAgent.Core;
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
        private DateTime _timeOfLastConnectionFailure;
        private Thread _connectionStatusMonitor;
        private const string AgentXmlTag = "TestAgentClient";
        private const string MultipleAgentsXmlTag = "TestAgentClients";
        private const string HostnameXmlTag = "TestAgentHostname";
        private const string PortXmlTag = "TestAgentPort";

        public event EventHandler<string> TestOutputReceived;

        protected virtual void OnTestOutputReceived(string e)
        {
            EventHandler<string> handler = TestOutputReceived;
            if (handler != null) handler(this, e);
        }

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
            ReconnectionWaitTime = 10000; // If the connection fails then retry after 10 seconds
            _timeOfLastConnectionFailure = DateTime.MinValue;
        }

        public void Connect()
        {
            if (IsConnected)
            {
                // We are already connected
                return;
            }

            // Do the connection on a separate thread to make sure we are not blocking the UI while connecting
            var connectionThread = new Thread(() =>
            {
                _fileClient = new FileServiceClientHost();
                _fileClient.Connect(Hostname, Port);

                _testClient = new TestServiceClientHost();
                _testClient.Connect(Hostname, Port);
                _testClient.OutputReceived += _testClient_OutputReceived;

                _isRunning = true;

                StartMonitoringConnectionStatus();
            });
            connectionThread.IsBackground = true;
            connectionThread.Start();
        }

        void _testClient_OutputReceived(object sender, string e)
        {
            OnTestOutputReceived(e);
        }

        private void StartMonitoringConnectionStatus()
        {
            if (_connectionStatusMonitor == null || !_connectionStatusMonitor.IsAlive)
            {
                _connectionStatusMonitor = new Thread(() =>
                {
                    while (_isRunning)
                    {
                        IsConnected = _fileClient.State == ConnectionState.Online &&
                                      _testClient.State == ConnectionState.Online;

                        Thread.Sleep(500);

                        if ((DateTime.Now - _timeOfLastConnectionFailure).TotalMilliseconds > ReconnectionWaitTime)
                        {
                            Connect();
                        }
                    }
                });
                _connectionStatusMonitor.IsBackground = true;
                _connectionStatusMonitor.Name = string.Format("{0}_{1}_Monitor", Hostname, Port);
                _connectionStatusMonitor.Start();
            }
        }

        public int ReconnectionWaitTime { get; set; }

        public bool IsConnected
        {
            get { return _isConnected; }
            private set
            {
                if (value == _isConnected)
                {
                    return;
                }

                if (!value)
                {
                    _timeOfLastConnectionFailure = DateTime.Now;
                }

                _isConnected = value;

                OnIsConnectedChanged();
            }
        }

        public string Hostname { get; private set; }

        public int Port { get; private set; }

        public FileServiceClientHost FileService { get { return _fileClient; } }

        public TestServiceClientHost TestService { get { return _testClient; } }

        public void Disconnect()
        {
            _fileClient.Disconnect();
            _testClient.Disconnect();
        }

        public void ExportToXml(XmlWriter writer)
        {
            writer.WriteStartElement(AgentXmlTag);
            writer.WriteXmlChild(HostnameXmlTag, Hostname);
            writer.WriteXmlChild(PortXmlTag, Port);
            writer.WriteEndElement();
        }

        public static TestAgentClient ImportFromXml(XmlReader reader)
        {
            bool isHostname = false;
            bool isPort = false;

            string hostname = string.Empty;
            int port = 0;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        isHostname = reader.Name == HostnameXmlTag;
                        isPort = reader.Name == PortXmlTag;
                        break;

                    case XmlNodeType.EndElement:
                        if (reader.Name == AgentXmlTag)
                        {
                            return new TestAgentClient(hostname, port);
                        }
                        break;

                    case XmlNodeType.Text:
                        if (isHostname)
                        {
                            hostname = reader.Value;
                        }
                        else if (isPort)
                        {
                            int.TryParse(reader.Value, out port);
                        }
                        break;
                }
            }

            // For some reason we didn't find our EndElement -> return null
            return null;
        }

        public static void ExportMultipleToXml(IEnumerable<TestAgentClient> clients, string filename)
        {
            using (var writer = XmlWriter.Create(filename))
            {
                ExportMultipleToXml(clients, writer);
            }
        }

        public static void ExportMultipleToXml(IEnumerable<TestAgentClient> clients, XmlWriter writer)
        {
            writer.WriteStartElement(MultipleAgentsXmlTag);
            foreach (var testAgentClient in clients)
            {
                testAgentClient.ExportToXml(writer);
            }
            writer.WriteEndElement();
        }

        public static TestAgentClient[] ImportMultipleFromXml(XmlReader reader)
        {
            var results = new List<TestAgentClient>();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == AgentXmlTag)
                        {
                            var client = ImportFromXml(reader);
                            if (client != null)
                            {
                                results.Add(client);
                            }
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (reader.Name == MultipleAgentsXmlTag)
                        {
                            return results.ToArray();
                        }
                        break;
                }
            }

            // We did not find the EndElement of the collection
            return new TestAgentClient[0];
        }

        public static TestAgentClient[] ImportMultipleFromXml(string filename)
        {
            using (var reader = XmlReader.Create(filename))
            {
                return ImportMultipleFromXml(reader);
            }
        }
    }
}
