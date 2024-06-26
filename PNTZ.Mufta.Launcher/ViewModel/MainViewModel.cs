using Promatis.DebuggingToolkit.IO;
using Promatis.Desktop.Control;
using Promatis.Desktop.MVVM;
using System.ComponentModel;

namespace PNTZ.Mufta.Launcher.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel(ICliUser cli)
        {
            CliViewModel = new CliViewModel(cli);
        }

        public CliViewModel CliViewModel { get; private set; }
    }
}
