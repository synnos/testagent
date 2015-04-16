﻿using System.Windows.Media;
using Caliburn.Micro;
using TestAgent.Services;

namespace TestAgent.Manager
{
    public class TestAgentViewModel : Screen
    {
        private TestAgentClient _client;
        private static SolidColorBrush _onlineColor = new SolidColorBrush(Color.FromRgb(200, 255, 0));
        private static SolidColorBrush _offlineColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        public TestAgentViewModel(TestAgentClient client)
        {
            _client = client;
            _client.IsConnectedChanged += _client_IsConnectedChanged;
        }

        void _client_IsConnectedChanged(object sender, System.EventArgs e)
        {
            NotifyOfPropertyChange(() => ConnectionStatus);
            NotifyOfPropertyChange(() => ConnectionStatusColor);
        }

        public string Hostname { get { return _client.Hostname; } }

        public string ConnectionStatus { get { return _client.IsConnected ? "Online" : "Offline"; } }

        public SolidColorBrush ConnectionStatusColor
        {
            get
            {
                return _client.IsConnected ? _onlineColor : _offlineColor;
            }
        }

        public void Connect()
        {
            _client.Connect();
        }
    }
}