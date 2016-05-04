using System;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using Gemini;
using Gemini.Framework;

namespace TestAgent.Modules
{
    [Export(typeof(IModule))]
    public class CoreModule : ModuleBase
    {
        [ImportingConstructor]
        public CoreModule()
        {
        }

        public override void Initialize()
        {
            // Set the application title
            MainWindow.Title = "Test Agent";
            MainWindow.Icon = new BitmapImage(new Uri("logo.ico", UriKind.Relative));

            if (ProjectManager != null)
            {
                ProjectManager.CurrentProjectChanged += (sender, args) =>
                {
                    if (ProjectManager.CurrentProject == null)
                    {
                        MainWindow.Title = "Test Agent";
                    }
                    else
                    {
                        string projectName = !string.IsNullOrEmpty(ProjectManager.CurrentProject.FileName) ? ProjectManager.CurrentProject.FileName
                            : !string.IsNullOrEmpty(ProjectManager.CurrentProject.Name) ? ProjectManager.CurrentProject.Name
                                : "Untitled";
                        MainWindow.Title = string.Format("{0} - Test Agent", projectName);
                    }
                };
            }
        }

        [Import]
        private IProjectManager ProjectManager { get; set; }
    }
}