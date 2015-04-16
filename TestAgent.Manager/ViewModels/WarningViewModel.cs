using Caliburn.Micro;

namespace TestAgent.Manager
{
    public class WarningViewModel : Screen
    {
        public WarningViewModel(string message, string title)
        {
            DisplayName = title;
            Message = message;
        }

        public string Message { get; private set; }

        public void Ok()
        {
            TryClose(true);
        }
    }
}