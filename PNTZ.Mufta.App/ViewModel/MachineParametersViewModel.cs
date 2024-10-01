using Desktop.MVVM;
using PNTZ.Mufta.App.Domain;

using System.Windows.Input;

using static PNTZ.Mufta.App.App;



namespace PNTZ.Mufta.App.ViewModel
{
    public class MachineParametersViewModel : BaseViewModel
    {

        public MachineParametersViewModel()
        {

            SavedMp =  AppInstance.OpenMachineParameters();

            SaveMpCommand = new RelayCommand((arg) =>  AppInstance.SaveMachineParameters(PlcMp));
        }

        public MachineParameters PlcMp { get; set; }
        public MachineParameters SavedMp { get; set; }

        public ICommand SaveMpCommand { get; set; }
    }
}
