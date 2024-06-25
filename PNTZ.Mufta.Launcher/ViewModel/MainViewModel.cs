using Promatis.DebuggingToolkit.IO;
using Promatis.Desktop.Control;
using System.ComponentModel;

namespace PNTZ.Mufta.Launcher.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel(ICliUser cli)
        {
            CliViewModel = new CliViewModel(cli);
        }

        public CliViewModel CliViewModel { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
