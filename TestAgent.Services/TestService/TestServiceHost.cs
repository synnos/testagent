using System;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.ServiceModel.Dispatcher;

namespace TestAgent.Services.TestService
{
    public class TestServiceHost
    {
        private readonly int _portNumber;
        private readonly TestService _service;
        private ServiceHost _host;

        public TestServiceHost(int portNumber, string uploadDirectory)
        {
            _portNumber = portNumber;
            _service = new TestService {UploadDirectory = uploadDirectory};
        }

        public TestService Service { get { return _service; } }

        public Uri ServiceAddress { get; private set; }

        public CommunicationState State { get { return _host == null ? CommunicationState.Closed : _host.State; } }

        public void LaunchService()
        {
            ServiceAddress = new Uri(string.Format("net.tcp://" + Environment.MachineName + ":{0}/TestAgent", _portNumber));

            _host = new ServiceHost(_service, ServiceAddress);

            var serviceDebugBehavior = _host.Description.Behaviors.Find<ServiceDebugBehavior>();
            serviceDebugBehavior.IncludeExceptionDetailInFaults = true;

            try
            {
                var netTcpBinding = new NetTcpBinding(SecurityMode.None);
                var endpoint = _host.AddServiceEndpoint(typeof(ITestService), netTcpBinding, "TestAgent");
                endpoint.Behaviors.Add(new ClientTrackerEndpointBehavior());

                var throttle = new ServiceThrottlingBehavior { MaxConcurrentSessions = 100 };
                _host.Description.Behaviors.Add(throttle);

                // Discovery
                var discoveryBehaviour = new ServiceDiscoveryBehavior();
                _host.Description.Behaviors.Add(discoveryBehaviour);
                discoveryBehaviour.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());

                _host.AddServiceEndpoint(new UdpDiscoveryEndpoint());

                // Metadata publishing.
                Binding mexTcpBinding = MetadataExchangeBindings.CreateMexTcpBinding();
                var behaviour = new ServiceMetadataBehavior();
                _host.Description.Behaviors.Add(behaviour);
                _host.AddServiceEndpoint(typeof(IMetadataExchange), mexTcpBinding,
                    string.Format("net.tcp://" + Environment.MachineName + ":{0}/TestAgent", _portNumber));

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

    [ExcludeFromCodeCoverage]
    class ClientTrackerChannelInitializer : IChannelInitializer
    {
        internal static int ConnectedClientCount = 0;

        public void Initialize(IClientChannel channel)
        {
            // TODO
        }
    }

    [ExcludeFromCodeCoverage]
    class ClientTrackerEndpointBehavior : IEndpointBehavior
    {
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.ChannelDispatcher.ChannelInitializers.Add(new ClientTrackerChannelInitializer());
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }
    }
}