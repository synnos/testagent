using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Framework;
using Gemini.Modules.MainMenu.Models;

namespace TestAgent.Modules
{
    [Export(typeof(IModule))]
    public class TestAgentConnectorModule : ModuleBase
    {
        public override void Initialize()
        {
            var fileMenu = MainMenu.All.First(x=>x.Name == "File");
            fileMenu.Children.Insert(0, new MenuItem("New"));
        }
    }
}
