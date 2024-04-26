using Promatis.Core;
using Promatis.Core.Logging;
using Promatis.ServiceModel.Interface;
using Promatis.ServiceModel.Interface.Settings;

namespace Promatis.MES.CLG.Console
{
    public class ModuleSettings : DataSyncModuleSettings
    {
        public ModuleSettings(string moduleName, ILogger logger) 
        {
            ModuleName = moduleName;
            InternalServicesCatalog = new DemoSyncServiceCatalog();
        }

    }
    internal sealed class DemoSyncServiceCatalog : IInternalServiceCatalog
    {
        public string Get(string serviceName)
        {
            return string.Empty;
        }
    }
}
