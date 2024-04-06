using GrpcServerSource.Interfaces;
using GrpcServerSource.Utilities;
using System.ComponentModel.Composition;
using System.Windows;

namespace GrpcTestingServer
{

    public partial class App : Application
    {
        [ImportMany]
        private List<IGrpcServerHelper> m_services = null;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MefManager.Initialize();
            MefManager.Container.ComposeParts(this);
            m_services.ForEach((x) => x.StartServer());
        }
    }
}
