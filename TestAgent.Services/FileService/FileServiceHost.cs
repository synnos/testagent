using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace TestAgent.Services.FileService
{
    public class FileServiceHost
    {
        private readonly int _portNumber;
        private readonly FileService _service;
        private ServiceHost _host;

        public FileServiceHost(int portNumber, string uploadDirectory)
        {
            _portNumber = portNumber;
            _service = new FileService {UploadDirectory = uploadDirectory};
        }

        public FileService Service { get { return _service; } }

        public Uri ServiceAddress { get; private set; }

        public CommunicationState State { get { return _host == null ? CommunicationState.Closed : _host.State; } }

        public void LaunchService()
        {
            ServiceAddress = new Uri(string.Format("net.tcp://" + Environment.MachineName + ":{0}/TestAgentFileTransfer", _portNumber));

            _host = new ServiceHost(_service, ServiceAddress);

            var serviceDebugBehavior = _host.Description.Behaviors.Find<ServiceDebugBehavior>();
            serviceDebugBehavior.IncludeExceptionDetailInFaults = true;

            try
            {
                var netTcpBinding = new NetTcpBinding(SecurityMode.None);
                netTcpBinding.TransferMode = TransferMode.Streamed;
                netTcpBinding.MaxReceivedMessageSize = 67108864;
                netTcpBinding.MaxBufferSize = 65536;
                _host.AddServiceEndpoint(typeof(IFileService), netTcpBinding, "TestAgentFileTransfer");

                var throttle = new ServiceThrottlingBehavior { MaxConcurrentSessions = 100 };
                _host.Description.Behaviors.Add(throttle);

                // Metadata publishing.
                Binding mexTcpBinding = MetadataExchangeBindings.CreateMexTcpBinding();
                var behaviour = new ServiceMetadataBehavior();
                _host.Description.Behaviors.Add(behaviour);
                _host.AddServiceEndpoint(typeof(IMetadataExchange), mexTcpBinding,
                    string.Format("net.tcp://" + Environment.MachineName + ":{0}/TestAgentFileTransfer", _portNumber));

                _host.Open();
            }
            catch (CommunicationException ce)
            {
                _host.Abort();
            }
        }

        public void CloseService()
        {
            if (_host != null)
            {
                _host.Close();
            }
        }
    }
}