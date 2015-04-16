using Caliburn.Micro;

namespace TestAgent.Manager
{
    public class AddAgentViewModel : Screen
    {
        private IWindowManager _windowManager;
        public AddAgentViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            DisplayName = "Add new test agent";
        }

        public string Hostname { get; set; }
        public int Port { get; set; }

        public void Add()
        {
            if (string.IsNullOrEmpty(Hostname))
            {
                _windowManager.ShowDialog(
                    new WarningViewModel("You must specify the hostname or IP address of the test agent!",
                        "No hostname specified!"));
                return;
            }

            if (Port <= 0)
            {
                _windowManager.ShowDialog(
                    new WarningViewModel("You must specify a valid port for the test agent!",
                        "Invalid port specified!"));
                return;
            }

            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }
    }
}