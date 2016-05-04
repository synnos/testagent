using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media.Imaging;
using Gemini.Framework;

namespace TestAgent.Modules
{
    [Export(typeof(IModule))]
    public class TestAgentConnectorModule : ModuleBase
    {
        public override void Initialize()
        {
        }
    }
}
