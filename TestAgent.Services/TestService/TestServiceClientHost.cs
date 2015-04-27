using System;
using System.ServiceModel;
using TestAgent.Core;

namespace TestAgent.Services.TestService
{
    public class TestServiceClientHost : ITestServiceCallback
    {
        public TestServiceClientHost()
        {
            State = ConnectionState.Offline;
        }

        public event EventHandler<string> OutputReceived;

        protected virtual void OnOutputReceived(string e)
        {
            EventHandler<string> handler = OutputReceived;
            if (handler != null) handler(this, e);
        }

        private DuplexChannelFactory<ITestService> _factory;

        public ITestService Client { get; private set; }

        public Uri ServiceAddress { get; private set; }

        public ConnectionState State { get; private set; }

        public void Connect(string hostname, int portNumber)
        {
            // Prepare connection objects and parameters.
            var binding = new NetTcpBinding(SecurityMode.None);

            ServiceAddress =
                new Uri(
                    string.Format(
                        "net.tcp://" + hostname + ":{0}/TestAgent/TestAgent",
                        portNumber));

            var serviceAddress = new EndpointAddress(ServiceAddress);
            
            _factory = new DuplexChannelFactory<ITestService>(this, binding, serviceAddress);
            _factory.Endpoint.Binding.OpenTimeout = TimeSpan.FromSeconds(20);
            _factory.Endpoint.Binding.ReceiveTimeout = TimeSpan.MaxValue;
            _factory.Endpoint.Binding.SendTimeout = TimeSpan.MaxValue;
            
            try
            {
                Client = _factory.CreateChannel();
                ((ICommunicationObject)Client).Faulted += ConnectionFaulted;
                ((ICommunicationObject)Client).Closed += ConnectionClosed;
                ((ICommunicationObject)Client).Opened += ConnectionOpened;

                Client.Register(Environment.MachineName);
            }
            catch (Exception ex)
            {
                State = ConnectionState.Offline;
                Console.WriteLine("Exception occured: \n{0}", ex);
            }
        }

        void ConnectionFaulted(object sender, EventArgs e)
        {
            State = ConnectionState.Offline;
        }

        void ConnectionClosed(object sender, EventArgs e)
        {
            State = ConnectionState.Offline;
        }

        void ConnectionOpened(object sender, EventArgs e)
        {
            State = ConnectionState.Online;
        }
        
        public void OnTestOutputChanged(string output)
        {
            OnOutputReceived(output);
        }

        public void OnTestFinished(TestResult result, string exceptionMessage, string exceptionStackTrace)
        {
            
        }

        public void OnTestRunFinished(TestResult result, string exceptionMessage, string exceptionStackTrace)
        {
        }

        public void OnTestRunStarted(string testFilename, int numberOfSelectedTests)
        {
        }

        public void OnTestStarted(string testName)
        {
        }

        public void OnUnhandledExceptionInTest(string exceptionMessage, string exceptionStackTrace)
        {
        }

        public void Disconnect()
        {
            if (_factory != null)
            {
                _factory.Close();
            }
        }
    }
}