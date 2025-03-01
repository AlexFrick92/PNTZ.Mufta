﻿using DpConnect;
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
        public IDpValue<string> MP_Machine_No { get; set; }
        public IDpValue<string> MP_Cal_User { get; set; }


        MachineParam actualMachineParam;
        public MachineParam ActualMachineParam
        {
            get => actualMachineParam ?? (actualMachineParam = new MachineParam());
            set 
            {
                actualMachineParam = value;
                MachineParamUpdate(this, value);
            }
        }
        public event EventHandler<MachineParam> MachineParamUpdate;

        CancellationTokenSource cts = null;
        bool MpProcedureStarted = false;


        bool cyclicallyListen = false;
        public bool CyclicallyListenMp
        {
            get => cyclicallyListen;
            set
            {
                if (!value)
                {
                    StopAwaitingForMp();                    
                }
                else
                {                    
                    if (!MpProcedureStarted && !CyclicallyListenMp)
                    {
                        logger.Info("Цикличное прослушивание параметров машин.");
                        if (DpTpcCommand.IsConnected)
                        {
                            StartAwaitingForMpAsync();
                        }
                        else
                        {                            
                            DpTpcCommand.StatusChanged += StartOnConnect;
                        }
                    }
                }
            }
        }

        private async void StartOnConnect(object sender, EventArgs e)
        {
            if(DpTpcCommand.IsConnected)
            {                                
                await StartAwaitingForMpAsync();
            }
        }
        private async void StartOnCommandUpdate(object sender, uint value)
        {
            logger.Info("Получена команда от ПЛК для параметров машин: " + value);
            await StartAwaitingForMpAsync();
        }
        private void StopOnDisconnect(object sender, EventArgs e)
        {


            if (!DpTpcCommand.IsConnected && CyclicallyListenMp)
            {
                StopAwaitingForMp();
                logger.Info("Прослушивание МП будет возобновлено при подключении");
                DpTpcCommand.StatusChanged += StartOnConnect;
            }

        }

        ILogger logger;
        public MachineParamFromPlc(ILogger logger)
        {
            this.logger = logger;
        }
        public void DpBound()
        {
            DpPlcCommand.StatusChanged += StopOnDisconnect;
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
                MP_Makeup_Length_Offset = MP_Makeup_Length_Offset.Value,
                MP_Machine_No = MP_Machine_No.Value,
                MP_Cal_User = MP_Cal_User.Value,
            };
        }

        private void StopAwaitingForMp()
        {
            cyclicallyListen = false;
            DpPlcCommand.ValueUpdated -= StartOnCommandUpdate;
            DpTpcCommand.StatusChanged -= StartOnConnect;
            logger.Info("Цикличное прослушивание МП остановлено");

            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
        }

        private async Task StartAwaitingForMpAsync()
        {
            cyclicallyListen = true;
            DpPlcCommand.ValueUpdated -= StartOnCommandUpdate;
            DpTpcCommand.StatusChanged -= StartOnConnect;

            await Task.Run(async () =>
            {
                if (MpProcedureStarted)
                    throw new InvalidOperationException("Прослушивание МП уже активировано!");

                logger.Info("Активировано прослушивание параметров машин. Ожидаем ПЛК!");
                while (CyclicallyListenMp)
                {
                    //await Task.Delay(TimeSpan.FromSeconds(2));

                    try
                    {                        
                        DpTpcCommand.Value = 0;
                        cts = new CancellationTokenSource();
                        MpProcedureStarted = true;
                        await MachineParamListen(cts.Token);
                        logger.Info("Параметры машин записаны.");
                    }
                    catch (OperationCanceledException)
                    {
                        logger.Info("Прослушивание параметров машин отменено.");
                    }
                    catch (InvalidOperationException ex)
                    {
                        logger.Info(ex.Message);
                        CyclicallyListenMp = false;
                        logger.Info("Прослушивание параметров будет возобновится после новой команды от ПЛК.");
                        DpPlcCommand.ValueUpdated += StartOnCommandUpdate;

                    }
                    catch (Exception ex)
                    {
                        logger.Info("Незивестная ошибка при попытке считать параметры");
                        logger.Info(ex.Message);
                        CyclicallyListenMp = false;
                    }
                    finally
                    {
                        if (DpTpcCommand.IsConnected)
                            DpTpcCommand.Value = 0;                                                                
                    }
                }
                logger.Info("Прослушивание параметров машин завершено");
                MpProcedureStarted = false;
            });
        }
        
        async Task MachineParamListen(CancellationToken token)
        {
            TaskCompletionSource<uint> awaitFor5 = null;
            Task timeout = null;
            Task first = null;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            token.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            
            //Ожидаем 5. 5 - новые параметры

            while(true)
            {
                awaitFor5 = new TaskCompletionSource<uint>();
                DpPlcCommand.ValueUpdated += (s, v) => awaitFor5.TrySetResult(v);

                first = await Task.WhenAny(awaitFor5.Task, tcs.Task);

                if (first == tcs.Task)
                {
                    throw new OperationCanceledException();
                }

                logger.Info("МП. команда ПЛК:" + awaitFor5.Task.Result);



                if (awaitFor5.Task.Result == 5)
                {
                    logger.Info("Новые параметры машин...");

                    ActualMachineParam = MakeMachineParam();

                    logger.Info(ActualMachineParam.ToString());
                    break;
                }
                else if (awaitFor5.Task.Result == 0)
                {
                    //Добавим здесь задежрку перед выходов в новый цикл.
                    //Без нее пораждаются множественные подписки. Причина пока не выявлена
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    continue;
                }
                else
                    throw new InvalidOperationException("Неверный ответ ПЛК. Ожидаем 5");

                
            }

            //Отправляем 10 и ждем 20
            DpTpcCommand.Value = 10;

            timeout = Task.Delay(TimeSpan.FromSeconds(10));
            var awaitFor20 = new TaskCompletionSource<uint>();

            DpPlcCommand.ValueUpdated += (s, v) => awaitFor20.TrySetResult(v);

            first =  await Task.WhenAny(awaitFor20.Task, timeout);

            if (first == timeout)           
                throw new TimeoutException("Время ожидания команды истекло");            

            logger.Info("МП. команда ПЛК:" + awaitFor20.Task.Result);

            if (awaitFor20.Task.Result != 20)
            {
                throw new InvalidOperationException("Неверная команда ПЛК. Ожидаем 20");
            }


            //Отправляем 40 и ждем 50.
            DpTpcCommand.Value = 40;

            timeout = Task.Delay(TimeSpan.FromSeconds(10));
            var awaitFor50 = new TaskCompletionSource<uint>();

            DpPlcCommand.ValueUpdated += (s, v) => awaitFor50.TrySetResult(v);

            first = await Task.WhenAny(awaitFor50.Task, timeout);
            if (first == timeout)
                throw new TimeoutException("Время ожидания команды истекло");

            logger.Info("МП. команда ПЛК:" + awaitFor50.Task.Result);

            if (awaitFor50.Task.Result != 50)
            {
                throw new InvalidOperationException("Неверная комана ПЛК. Ожидаем 50");
            }


            //Отправляем 0
            DpTpcCommand.Value = 0;

            timeout = Task.Delay(TimeSpan.FromSeconds(10));
            var awaitFor0 = new TaskCompletionSource<uint>();

            DpPlcCommand.ValueUpdated += (s, v) => awaitFor0.TrySetResult(v);

            first = await Task.WhenAny(awaitFor0.Task, timeout);
            if (first == timeout)
                throw new TimeoutException("Время ожидания команды истекло");

            logger.Info("МП. команда ПЛК:" + awaitFor0.Task.Result);

            if (awaitFor0.Task.Result != 0)
            {
                throw new InvalidOperationException("Неверная комана ПЛК. Ожидаем 0");
            }

        }
    }
}
