using System;
using System.Threading;
using System.Threading.Tasks;
using DpConnect.Interface;

using static PNTZ.Mufta.App.App;

namespace PNTZ.Mufta.App.Domain
{
    public class MachineParameterObserver : DpProcessor
    {

        public MachineParameterObserver()
        {
            DpInitialized += MachineParameterObserver_DpInitialized;
            DpInitialized += MachineParameterObserver_DpInitialized1;                
        }

        private void MachineParameterObserver_DpInitialized1(object sender, EventArgs e)
        {
            Console.WriteLine(CommandFeedback.Value);
            CommandFeedback.ValueUpdated += CommandFeedback_ValueUpdated;
        }

        private void CommandFeedback_ValueUpdated(object sender, uint e)
        {
            Console.WriteLine(e);
        }

        private void MachineParameterObserver_DpInitialized(object sender, EventArgs e)
        {
            CancellationTokenSource ctc = null;
            AppInstance.AppCli.RegisterCommand("startmp", async (arg) =>
            {
                AppInstance.AppCli.WriteLine("Активировано прослушивание параметров машин. Ожидаем ПЛК!");
                try
                {
                    ctc = new CancellationTokenSource();
                    await MachineParamListen(ctc.Token);
                    AppInstance.AppCli.WriteLine("Параметры машин записаны.");
                }
                catch (Exception ex)
                {
                    SetMPCommand.Value = 0;
                    AppInstance.AppCli.WriteLine(ex.Message);
                }
                AppInstance.AppCli.WriteLine("Прослушивание параметров машин завершено");
            });
            AppInstance.AppCli.RegisterCommand("stopmp", (arg) =>
            {
                if(ctc != null)
                {
                    ctc.Cancel();
                    ctc = null;
                }                
            });

            //ObservableMachineParameters.ValueUpdated += ObservableMachineParameters_ValueUpdated;
        }

        async Task MachineParamListen(CancellationToken token)
        {
            if(CommandFeedback.Value != 0)
            {
                throw new Exception("Перед началом операции команда ПЛК должна быть 0. Сейчас - " + CommandFeedback.Value );
            }

            TaskCompletionSource<uint> awaitCommandFeedback = new TaskCompletionSource<uint>();

            CommandFeedback.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);            

            await awaitCommandFeedback.Task;

            AppInstance.AppCli.WriteLine("МП. команда ПЛК:" + awaitCommandFeedback.Task.Result);

            if (awaitCommandFeedback.Task.Result != 5)
            {
                throw new Exception("Неверный ответ ПЛК. Ожидаем 5");
            }

            var mp = ObservableMachineParameters.Value;
            
            AppInstance.AppCli.WriteLine("Считаны параметры машин!");
            AppInstance.AppCli.WriteLine("MP_Load_Cell_Span: " + mp.MP_Load_Cell_Span.ToString());
            AppInstance.AppCli.WriteLine("MP_Load_Span_Digits: " + mp.MP_Load_Span_Digits.ToString());
            AppInstance.AppCli.WriteLine("MP_Handle_Length: " + mp.MP_Handle_Length.ToString());
            AppInstance.AppCli.WriteLine("MP_Handle_Length_Digits: " + mp.MP_Handle_Length_Digits.ToString());
            AppInstance.AppCli.WriteLine("MP_TC_PPR: " + mp.MP_TC_PPR.ToString());
            AppInstance.AppCli.WriteLine("MP_Box_Length: " + mp.MP_Box_Length.ToString());
            AppInstance.AppCli.WriteLine("MP_Box_Length_Digit: " + mp.MP_Box_Length_Digit.ToString());
            AppInstance.AppCli.WriteLine("MP_Makeup_Length: " + mp.MP_Makeup_Length.ToString());
            AppInstance.AppCli.WriteLine("MP_Makeup_Length_Digits: " + mp.MP_Makeup_Length_Digits.ToString());
            AppInstance.AppCli.WriteLine("MP_Tq_Max: " + mp.MP_Tq_Max.ToString());
            AppInstance.AppCli.WriteLine("MP_Machine_No: " + mp.MP_Machine_No.ToString());
            AppInstance.AppCli.WriteLine("MP_Cal_Factor: " + mp.MP_Cal_Factor.ToString());
            AppInstance.AppCli.WriteLine("MP_Cal_User: " + mp.MP_Cal_User.ToString());
            AppInstance.AppCli.WriteLine("MP_Cal_Timestamp: " + mp.MP_Cal_Timestamp.ToString());
            AppInstance.AppCli.WriteLine("MP_Makeup_Length_Offset: " + mp.MP_Makeup_Length_Offset.ToString());

            SetMPCommand.Value = 10;

            awaitCommandFeedback = new TaskCompletionSource<uint>();

            CommandFeedback.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            await awaitCommandFeedback.Task;

            AppInstance.AppCli.WriteLine("МП. команда ПЛК:" + awaitCommandFeedback.Task.Result);

            if (awaitCommandFeedback.Task.Result != 20)
            {
                throw new Exception("Неверная команда ПЛК. Ожидаем 20");
            }

            SetMPCommand.Value = 40;

            awaitCommandFeedback = new TaskCompletionSource<uint>();

            CommandFeedback.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            await awaitCommandFeedback.Task;

            AppInstance.AppCli.WriteLine("МП. команда ПЛК:" + awaitCommandFeedback.Task.Result);

            if (awaitCommandFeedback.Task.Result != 50)
            {
                throw new Exception("Неверная комана ПЛК. Ожидаем 50");
            }

            SetMPCommand.Value = 0;
        }

        public IDpValue<MachineParameters> ObservableMachineParameters { get; set; }

        public IDpValue<uint> SetMPCommand { get; set; }

        public IDpValue<uint> CommandFeedback { get; set; }
    }
}
