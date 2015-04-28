using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace TestAgent.Manager
{
    public class OpenViewModel : Screen
    {
        private string _testFile;
        private string[] _dependencies;

        public OpenViewModel()
        {
            DisplayName = "Open tests";
        }

        public void SelectDependencies()
        {
            var uploadDirectoryPicker = new CommonOpenFileDialog
            {
                Title = "Chooose your test dependencies",
                EnsurePathExists = true,
                Multiselect = true
            };

            if (uploadDirectoryPicker.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok)
            {
                Dependencies = uploadDirectoryPicker.FileNames.ToArray();
            }
        }

        public void SelectTest()
        {
            var uploadDirectoryPicker = new CommonOpenFileDialog
            {
                Title = "Chooose your test file",
                EnsurePathExists = true,
                Multiselect = false
            };
            uploadDirectoryPicker.Filters.Add(new CommonFileDialogFilter("NUnit Test Dlls", "*.dll"));

            if (uploadDirectoryPicker.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok)
            {
                TestFile = uploadDirectoryPicker.FileName;
            }
        }

        public void Ok()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }

        public string[] Dependencies
        {
            get { return _dependencies; }
            private set
            {
                if (Equals(value, _dependencies)) return;
                _dependencies = value;
                NotifyOfPropertyChange(() => Dependencies);
            }
        }

        public string TestFile
        {
            get { return _testFile; }
            set
            {
                if (value == _testFile) return;
                _testFile = value;
                NotifyOfPropertyChange(() => TestFile);
            }
        }
    }
}