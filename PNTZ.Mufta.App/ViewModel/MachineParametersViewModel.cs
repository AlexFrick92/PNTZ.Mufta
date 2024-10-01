using Desktop.MVVM;
using PNTZ.Mufta.App.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.App.ViewModel
{
    public class MachineParametersViewModel : BaseViewModel
    {
        public MachineParameters PlcMp { get; set; }
        public MachineParameters SavedMp { get; set; }
    }
}
