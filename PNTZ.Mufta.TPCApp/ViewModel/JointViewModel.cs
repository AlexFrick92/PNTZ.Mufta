using Desktop.MVVM;

using PNTZ.Mufta.TPCApp.DpConnect;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static PNTZ.Mufta.TPCApp.App;

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

                    await Task.Delay(UpdateInterval);
                }
            });
            jointOperationalParam.DpParam.ValueUpdated -= SubscribeToValues;
        }

        public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMilliseconds(100);

        public JointViewModel(JointOperationalParam jointParam, ILogger logger)
        {
            this.logger = logger;

            try
            {
                var config = XDocument.Load($"{AppInstance.CurrentDirectory}/ViewModel/JointViewModel.xml");
                UpdateInterval = TimeSpan.FromMilliseconds(int.Parse(config.Root.Element("JointOperationParam").Attribute("UpdateInterval").Value));
            }
            catch (Exception ex)
            {
                logger.Info("Не удалось загрузить конфигурацию для JointViewModel:");
                logger.Info(ex.Message);
                logger.Info("Будут использованы значения по-умолчанию");
            }

            this.JointOperationalParam = jointParam;
        }
    }
}

