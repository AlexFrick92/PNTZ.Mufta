using Desktop.MVVM;



using PNTZ.Mufta.TPCApp.DpConnect;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class JointViewModel : BaseViewModel
    {

        public double ActualTorque { get; set; } = 0;
        public double ActualLength { get; set; } = 0;
        public double ActualTurns { get; set; } = 0;


        ILogger logger;
        JointOperationalParam jointOperationalParam;
        JointOperationalParam JointOperationalParam
        {
            get => jointOperationalParam;
            set
            {
                if(value == null)
                    throw new ArgumentNullException(nameof(value));

                jointOperationalParam = value;

                jointOperationalParam.DpParam.ValueUpdated += SubscribeToValues;
            }
        }

        private void SubscribeToValues(object sender, DpConnect.Struct.OperationalParam e)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    ActualTorque = jointOperationalParam.DpParam.Value.Torque;
                    ActualLength = jointOperationalParam.DpParam.Value.Length;
                    ActualTurns = jointOperationalParam.DpParam.Value.Turns;

                    OnPropertyChanged(nameof(ActualTorque));
                    OnPropertyChanged(nameof(ActualLength));
                    OnPropertyChanged(nameof(ActualTurns));

                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }
            });
            jointOperationalParam.DpParam.ValueUpdated -= SubscribeToValues;
        }

        public JointViewModel(JointOperationalParam jointParam, ILogger logger)
        {
            this.logger = logger;
            this.JointOperationalParam = jointParam;
        }
    }
}

