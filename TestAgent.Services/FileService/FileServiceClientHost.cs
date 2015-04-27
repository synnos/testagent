using System;
using System.ServiceModel;

namespace TestAgent.Services.FileService
{
    public class FileServiceClientHost
    {
        private ChannelFactory<IFileService> _factory;

        public IFileService Client { get; private set; }

        public Uri ServiceAddress { get; private set; }

        public ConnectionState State { get { return _factory == null ? ConnectionState.Offline : ConnectionState.Online; } }

        public void Connect(string hostname, int portNumber)
        {
            // Prepare connection objects and parameters.
            var binding = new NetTcpBinding(SecurityMode.None);
            binding.MaxReceivedMessageSize = 2147483647;
            binding.TransferMode = TransferMode.Streamed;

            ServiceAddress =
                new Uri(
                    string.Format(
                        "net.tcp://" + hostname + ":{0}/TestAgentFileTransfer/TestAgentFileTransfer",
                        portNumber));

            var serviceAddress = new EndpointAddress(ServiceAddress);

            _factory = new ChannelFactory<IFileService>(binding, serviceAddress);

            try
            {
                Client = _factory.CreateChannel();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: \n{0}", ex);
            }
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