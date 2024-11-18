
using Desktop.Control;
using Desktop.MVVM;

using DpConnect;

using Toolkit.IO;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    internal class MainViewModel : BaseViewModel, IMainViewModel
    {
        IDpWorkerManager WorkerManager { get; set; }
        IDpConnectionManager ConnectionManager { get; set; }

        public CliViewModel CliViewModel { get; set; }

        public MainViewModel(IDpWorkerManager workerManager, IDpConnectionManager connectionManager, ICliProgram cli, ICliUser cliUI)
        {
            this.WorkerManager = workerManager;
            this.ConnectionManager = connectionManager;

            CliViewModel = new CliViewModel(cliUI);
        }
    }
}
