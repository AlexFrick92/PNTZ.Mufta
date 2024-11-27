using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.IO;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    internal class MachinParamViewModel : BaseViewModel
    {
        public MachineParam SavedMp { get; set; }
        public MachineParam PlcMp { get; set; }


        ICliProgram cliProgram { get; set; }

        MachineParamFromPlc mpListener;
        MachineParamFromPlc MpListener 
        {
            get
            {
                if (mpListener != null)
                    return mpListener;
                else
                    throw new ArgumentNullException("Прослушиватель параметров не инициализирован");
            }

            set
            {
                if (value != null)
                    mpListener = value;
                else
                    throw new ArgumentNullException("Прослушиватель параметров не инициализирован");


                cliProgram.RegisterCommand("startmp", (arg) => mpListener.CyclicallyListenMp = true);
                cliProgram.RegisterCommand("stopmp", (arg) => mpListener.CyclicallyListenMp = false);

                mpListener.MachineParamUpdate += (s, mp) =>
                {
                    PlcMp = mp;
                    OnPropertyChanged(nameof(PlcMp));
                };
                mpListener.CyclicallyListenMp = true;
            }
        }

        public MachinParamViewModel(MachineParamFromPlc paramFromPlc, ICliProgram cliProgram)
        {
            this.cliProgram = cliProgram;
            
            this.MpListener = paramFromPlc;
        }

    }
}
