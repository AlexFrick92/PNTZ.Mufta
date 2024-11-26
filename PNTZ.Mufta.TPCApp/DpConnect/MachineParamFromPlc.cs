using DpConnect;
using PNTZ.Mufta.TPCApp.Domain;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolkit.IO;

namespace PNTZ.Mufta.TPCApp.DpConnect
{
    public class MachineParamFromPlc : IDpWorker
    {
        public IDpValue<uint> DpTpcCommand { get; set; }
        public IDpValue<uint> DpPlcCommand { get; set; }

        public IDpValue<float> MP_Load_Cell_Span { get; set; }
        public IDpValue<float> MP_Load_Span_Digits { get; set; }
        public IDpValue<float> MP_Handle_Length { get; set; }
        public IDpValue<float> MP_Handle_Length_Digits { get; set; }
        public IDpValue<float> MP_TC_PPR {  get; set; }
        public IDpValue<float> MP_Box_Length { get; set; }
        public IDpValue<float> MP_Box_Length_Digit { get; set; }    
        public IDpValue<float> MP_Makeup_Length { get; set; }
        public IDpValue<float> MP_Makeup_Length_Digits { get; set; }
        public IDpValue<float> MP_Tq_Max {  get; set; }        
        public IDpValue<float> MP_Cal_Factor { get; set; }        
        public IDpValue<DateTime> MP_Cal_Timestamp { get; set; }
        public IDpValue<float> MP_Makeup_Length_Offset { get; set; }

        public MachineParam ActualMachineParam { get; set; }


        CancellationTokenSource cts;
        bool MpProcedureStarted = false;

        ILogger logger;
        ICliProgram cliProgram;
        public MachineParamFromPlc(ILogger logger, ICliProgram cliProgram)
        {
            this.logger = logger;
            this.cliProgram = cliProgram;
        }
        public void DpBound()
        {
            CancellationTokenSource ctc = null;
            cliProgram.RegisterCommand("startmp", async (arg) =>
            {

                if (MpProcedureStarted)
                    throw new Exception("Прослушивание МП уже активировано!");
                    
                logger.Info("Активировано прослушивание параметров машин. Ожидаем ПЛК!");
                try
                {
                    DpTpcCommand.Value = 0;
                    ctc = new CancellationTokenSource();
                    MpProcedureStarted = true;
                    await MachineParamListen(ctc.Token);
                    logger.Info("Параметры машин записаны.");
                }
                catch (Exception ex)
                {                    
                    logger.Info(ex.Message);
                }
                finally
                {                    
                    if(DpTpcCommand.IsConnected)
                        DpTpcCommand.Value = 0;
                    MpProcedureStarted = false;
                }
                logger.Info("Прослушивание параметров машин завершено");
            });
            cliProgram.RegisterCommand("stopmp", (arg) =>
            {
                if (ctc != null)
                {
                    ctc.Cancel();
                    ctc = null;
                }
            });
        }

        private MachineParam MakeMachineParam()
        {
            return new MachineParam()
            {
                MP_Load_Cell_Span = MP_Load_Cell_Span.Value,
                MP_Load_Span_Digits = MP_Load_Span_Digits.Value,
                MP_Handle_Length = MP_Handle_Length.Value,
                MP_Handle_Length_Digits = MP_Handle_Length_Digits.Value,
                MP_TC_PPR = MP_TC_PPR.Value,
                MP_Box_Length = MP_Box_Length.Value,
                MP_Box_Length_Digit = MP_Box_Length_Digit.Value,
                MP_Makeup_Length = MP_Makeup_Length.Value,
                MP_Makeup_Length_Digits = MP_Makeup_Length_Digits.Value,
                MP_Tq_Max = MP_Tq_Max.Value,
                MP_Cal_Factor = MP_Cal_Factor.Value,
                MP_Cal_Timestamp = MP_Cal_Timestamp.Value,
                MP_Makeup_Length_Offset = MP_Makeup_Length_Offset.Value
            };
        }
        async Task MachineParamListen(CancellationToken token)
        {
            if (DpPlcCommand.Value != 0)
            {
                throw new Exception("Перед началом операции команда ПЛК должна быть 0. Сейчас - " + DpPlcCommand.Value);
            }

            TaskCompletionSource<uint> awaitCommandFeedback = new TaskCompletionSource<uint>();

            DpPlcCommand.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            await awaitCommandFeedback.Task;

            logger.Info("МП. команда ПЛК:" + awaitCommandFeedback.Task.Result);

            if (awaitCommandFeedback.Task.Result != 5)
            {
                throw new Exception("Неверный ответ ПЛК. Ожидаем 5");
            }

            ActualMachineParam = MakeMachineParam();

            logger.Info(ActualMachineParam.ToString());

            DpTpcCommand.Value = 10;

            awaitCommandFeedback = new TaskCompletionSource<uint>();

            DpPlcCommand.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            await awaitCommandFeedback.Task;

            logger.Info("МП. команда ПЛК:" + awaitCommandFeedback.Task.Result);

            if (awaitCommandFeedback.Task.Result != 20)
            {
                throw new Exception("Неверная команда ПЛК. Ожидаем 20");
            }

            DpTpcCommand.Value = 40;

            awaitCommandFeedback = new TaskCompletionSource<uint>();

            DpPlcCommand.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            await awaitCommandFeedback.Task;

            logger.Info("МП. команда ПЛК:" + awaitCommandFeedback.Task.Result);

            if (awaitCommandFeedback.Task.Result != 50)
            {
                throw new Exception("Неверная комана ПЛК. Ожидаем 50");
            }

            DpTpcCommand.Value = 0;
        }
    }
}
