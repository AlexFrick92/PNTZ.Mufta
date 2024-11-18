using Desktop.Application;
using DpConnect;
using DpConnect.Connection;
using DpConnect.OpcUa;

using PNTZ.Mufta.TPCApp.View;

using Promatis.Core;
using Promatis.IoC.DryIoc;


namespace PNTZ.Mufta.TPCApp
{
    internal class App : StagedApplication
    {
        protected override void BeforeInit()
        {
            IIoCContainer container = new DryIocContainer();

            container.RegisterInstance(container);

            container.Register<IOpcUaConnection, OpcUaConnection>();
            container.Register<IDpConnectionManager, ContainerizedConnectionManager>();

            

        }

        protected override void Init()
        {
            mainWindow = new MainView();
        }

        protected override void AfterInit() 
        {
            
        }
    }
}
