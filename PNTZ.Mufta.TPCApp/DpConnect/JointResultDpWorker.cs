
using DpConnect;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect.Struct;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.DpConnect
{
    public class JointResultDpWorker : IDpWorker
    {
        public JointResultDpWorker(ILogger logger)
        {
            this.logger = logger;
        }

        ILogger logger;




        //Точки Dp - привязаны к OpcUa
        public IDpValue<uint> DpTpcCommand { get; set; }
        public IDpValue<uint> DpPlcCommand { get; set; }
        public IDpValue<OperationalParam> DpParam { get; set; }
        public IDpValue<ERG_CAM> Dp_ERG_CAM { get; set; }
        public IDpValue<ERG_Muffe> Dp_ERG_Muffe { get; set; }
        public IDpValue<ERG_MVS> Dp_ERG_MVS { get; set; }
        public void DpBound()
        {            
        }


        public TimeSpan CommandAwaitTimeout { get; set; } = TimeSpan.FromSeconds(60);
        public TimeSpan RecordingTimeout { get; set; } = TimeSpan.FromSeconds(60);

        //События соединения

        //Труба появилась на станке. 
        public event EventHandler<EventArgs> PipeAppear;
        //Труба в навёрточной головке. Началось свинчивания
        public event EventHandler<EventArgs> RecordingBegun;
        public event EventHandler<EventArgs> RecordingFinished;
        //Ожидание оценки оператором
        public event EventHandler AwaitForEvaluation;       
        //Свинчивание завершено
        public event EventHandler<JointResult> JointFinished;        
        

        //Процедура прослушивания запущена. Да, по этому флагу я определяю, можно ли запустить прослушнку. Конечно тут нужен lock...
        bool JointProcedureStarted = false;
        CancellationTokenSource cts = null;

        //Цикличный таск прослушивания процедуры свинчивания
        bool cyclicallyListen = false;

        //Свойство устанаилвается снаружи
        public bool CyclicallyListen
        {
            get => cyclicallyListen;
            set
            {
                if (!value)
                {
                    StopAwaiting();
                }
                else
                {
                    if (!JointProcedureStarted && !CyclicallyListen)
                    {
                        logger.Info("Цикличное прослушивание операции соединения.");
                        if (DpPlcCommand.IsConnected)
                        {
                            _ = StartProcedureAsync();
                        }
                        else
                        {
                            DpPlcCommand.StatusChanged += StartOnConnect;
                        }
                    }
                }
            }
        }


        private readonly object _lock = new object();   

        //При пропаже соединения останавливаем прослушку. Если процедура уже начата, то не будет эффекта, но она отвалится по таймауту
        private void StopOnDisconnect(object sender, EventArgs e)
        {
            if (!DpPlcCommand.IsConnected)
            {
                StopAwaiting();
                logger.Info("Прослушивание операции соединения будет возобновлено при подключении");
                DpPlcCommand.StatusChanged += StartOnConnect;
            }

        }

        //При появлении соединения начинаем прослушивать
        private void StartOnConnect(object sender, EventArgs e)
        {
            if (DpPlcCommand.IsConnected)
            {
                
                _ = StartProcedureAsync();
            }
        }

        //Начинаем прослушивать, если изменилась команда от ПЛК
        private void StartOnCommandUpdate(object sender, uint value)
        {
            logger.Info("Получена команда от ПЛК для операции соединения: " + value);            
            _ = StartProcedureAsync();
        }

        //Вход в таск прослушивания
        private async Task StartProcedureAsync()
        {
            lock(_lock)
            {
                if (JointProcedureStarted)
                    throw new InvalidOperationException("Прослушивание операции соединения уже активировано!");                

                JointProcedureStarted = true;
            }
            cyclicallyListen = true;
            DpPlcCommand.StatusChanged -= StartOnConnect;
            DpPlcCommand.ValueUpdated -= StartOnCommandUpdate;
            DpPlcCommand.StatusChanged += StopOnDisconnect;

            logger.Info("Активировано Прослушивание операции соединения. Ожидаем ПЛК!");

            while (cyclicallyListen)
            {
                try
                {
                    DpTpcCommand.Value = 0;
                    cts = new CancellationTokenSource();                    
                    await AwaitForJointProcess(cts.Token);
                    logger.Info("Соединение записано.");
                }
                catch (OperationCanceledException ex)
                {
                    logger.Info("Прослушивание операции соединения отменено");
                    cyclicallyListen = false;
                }
                catch (InvalidOperationException ex)
                {
                    logger.Info(ex.Message);
                    cyclicallyListen = false;
                    logger.Info("Прослушивание операции соединения возобновится после новой команды от ПЛК.");
                    DpPlcCommand.ValueUpdated += StartOnCommandUpdate;
                }
                catch (InvalidProgramException ex)
                {
                    logger.Info("Joint. Операция прервана ПЛК. Запускаем еще раз");                    
                }
                catch (TimeoutException ex)
                {
                    logger.Info(ex.Message);
                    cyclicallyListen = false;
                    logger.Info("Прослушивание операции соединения возобновится после новой команды от ПЛК.");
                    DpPlcCommand.ValueUpdated += StartOnCommandUpdate;
                }
                catch (Exception ex)
                {
                    logger.Info("Незивестная ошибка записи операции соединения");
                    logger.Info(ex.Message);
                    cyclicallyListen = false;
                }
                finally
                {
                    if (DpTpcCommand.IsConnected)
                        DpTpcCommand.Value = 0;

                    JointFinished?.Invoke(this, GetResult());
                }               
            }
                JointProcedureStarted = false;
        }

        //Отменить ожидание команды от ПЛК для нового соединения
        private void StopAwaiting()
        {
            cyclicallyListen = false;
            DpPlcCommand.ValueUpdated -= StartOnCommandUpdate;
            DpPlcCommand.StatusChanged -= StartOnConnect;
            DpPlcCommand.StatusChanged -= StopOnDisconnect;
            logger.Info("Цикличное прослушивание операции соединения остановлено");

            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
        }

        //Процедура записи соединения. Алгоритм обмена командами.
        //Ожидание можно прервать при помощи ct вначале. 
        //Далее, ожидание каждой команды ограничено таймаутом
        private async Task AwaitForJointProcess(CancellationToken token)
        {
            //Создаём токены для ожидания
            TaskCompletionSource<uint> awaitCommandFeedback = null;
            Task timeout = null;
            Task first = null;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            token.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);

            //Ожидаем команду 10. Цикл - если пришла 0 по подписке, хотя и так 0
            //Команда 10 - свинчивание начинается. Труба подводится к навёрточной головке


            while (true)
            {
                awaitCommandFeedback = new TaskCompletionSource<uint>();
                DpPlcCommand.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

                first = await Task.WhenAny(awaitCommandFeedback.Task, tcs.Task);

                if (first == tcs.Task)
                {
                    throw new OperationCanceledException();
                }


                logger.Info("Joint. команда ПЛК:" + awaitCommandFeedback.Task.Result);

                if (awaitCommandFeedback.Task.Result == 10)
                {
                    logger.Info("Труба появилась на позиции муфтонавёртки");
                    PipeAppear?.Invoke(this, EventArgs.Empty);
                    JointResult = new JointResult()
                    {
                        StartTimeStamp = DateTime.Now,
                    };
                    break;
                }
                else if (awaitCommandFeedback.Task.Result == 0)
                {
                    continue;
                }
                else
                    throw new InvalidOperationException("Неверный ответ от ПЛК. Ожидалось 10");

            }



            //Отправляем 20 и ждем 30 или 28 - Либо успешно, либо ошибка преднавёртки
            DpTpcCommand.Value = 20;

            timeout = Task.Delay(CommandAwaitTimeout);
            var AwaitFor30 = new TaskCompletionSource<uint>();

            DpPlcCommand.ValueUpdated += (s, v) => AwaitFor30.TrySetResult(v);

            first = await Task.WhenAny(AwaitFor30.Task, timeout, tcs.Task);

            if (first == timeout)
                throw new TimeoutException("Время ожидания команды истекло");
            if (first == tcs.Task)
            {
                throw new OperationCanceledException();
            }

            logger.Info("Joint. команда ПЛК:" + AwaitFor30.Task.Result);

            if (AwaitFor30.Task.Result != 30 && AwaitFor30.Task.Result != 28)            
                if (AwaitFor30.Task.Result == 0)
                    throw new InvalidProgramException();
                else
                    throw new InvalidOperationException($"Неверный ответ от ПЛК: Ожидалось 30 или 28");
            
            if (AwaitFor30.Task.Result == 28)
            {
                logger.Info("Joint. Ошибка преднавёртки!");
                throw new InvalidOperationException("Ошибка преднавёртки");
            }




            //Труба в позиции. Запускаем таск записи параметров

            logger.Info("Joint. Труба в позиции головки свинчивания. Готовимся к записи параметров!");

            CancellationTokenSource recordCtc = new CancellationTokenSource();
            //Отмена по таймауту. Предполагаю что monitoring time.
            timeout = Task.Run(async () =>
            {
                await Task.Delay(RecordingTimeout);
                recordCtc?.Cancel();
            });

            var recordTask = RecordOperationParams(recordCtc.Token);
            first = await Task.WhenAny(recordTask, timeout, tcs.Task);

            RecordingFinished?.Invoke(this, EventArgs.Empty);

            if (first == timeout)
                throw new TimeoutException("Время записи параметров истекло");
            if (first == tcs.Task)
            {
                recordCtc?.Cancel();
                throw new OperationCanceledException();
            }




            //Запись параметров закончена

            logger.Info("Как будто записали параметры");
            var AwaitFor40 = new TaskCompletionSource<uint>();
            timeout = Task.Delay(CommandAwaitTimeout);
            DpPlcCommand.ValueUpdated += (s, v) => AwaitFor40.TrySetResult(v);

            //Устанавливаем 38 - ответ записали параметры
            DpTpcCommand.Value = 38;

            first = await Task.WhenAny(AwaitFor40.Task, timeout, tcs.Task);

            if (first == timeout)
                throw new TimeoutException("Время ожидания команды истекло");
            if (first == tcs.Task)
            {
                throw new OperationCanceledException();
            }


            logger.Info("Joint. команда ПЛК:" + AwaitFor40.Task.Result);
            if (AwaitFor40.Task.Result != 40)
                if (AwaitFor40.Task.Result == 0)
                    throw new InvalidProgramException();
            else
                throw new InvalidOperationException($"Неверный ответ от ПЛК. Ожидалось 40");


            logger.Info("Итоговый момент: " + Dp_ERG_CAM.Value.PMR_MR_MAKEUP_FIN_TQ);
            logger.Info("Результат ПЛК:");

            logger.Info("Ожидаем оценки оператора");




            //Ожидается оценка
            TaskCompletionSource<uint> awaitEvaluation = new TaskCompletionSource<uint>();
            Evaluated += (s, v) => awaitEvaluation.TrySetResult(v);
            AwaitForEvaluation?.Invoke(null, EventArgs.Empty);

            await awaitEvaluation.Task;

            logger.Info("Оценка установлена: " + awaitEvaluation.Task.Result);

            var Value = awaitEvaluation.Task.Result;

            //jointResult.ResultTotal = awaitEvaluation.Task.Result;




            //Ожидаем завершения процедуры
            timeout = Task.Delay(CommandAwaitTimeout);
            var awaitFor0 = new TaskCompletionSource<uint>();
            DpPlcCommand.ValueUpdated += (s, v) => awaitFor0.TrySetResult(v);

            //Устанавливаем 50 - отправили оценку
            DpTpcCommand.Value = 50;

            first = await Task.WhenAny(awaitFor0.Task, timeout, tcs.Task);
            if (first == timeout)
                throw new TimeoutException("Время ожидания команды истекло");
            if (first == tcs.Task)
            {
                throw new OperationCanceledException();
            }

            logger.Info("Joint. команда ПЛК:" + awaitFor0.Task.Result);
            if (awaitFor0.Task.Result != 0)
            {
                throw new InvalidOperationException($"Неверный ответ от ПЛК. Ожидалось 0");
            }
        }

        async Task RecordOperationParams(CancellationToken token)
        {
            try
            {
                await Task.Run(async () =>
                {
                    bool started = false;

                    int ensureEnd = 0;

                    while (true)
                    {
                        if (!started)
                        {
                            if (DpParam.Value.Torque > 100)
                            {
                                started = true;
                                logger.Info("Регистрация параметров начата!");
                                RecordingBeginTimeStamp = DateTime.Now;
                                JointResult.Series = new List<TqTnLenPoint>();
                                DpParam.ValueUpdated += ActualTqTnLen_ValueUpdated;
                                RecordingBegun?.Invoke(this, EventArgs.Empty);
                            }
                        }
                        else
                        {
                            if (DpParam.Value.Torque < 1)
                            {
                                if (ensureEnd > 5)
                                {
                                    DpParam.ValueUpdated -= ActualTqTnLen_ValueUpdated;
                                    break;
                                }
                                else
                                    ensureEnd++;
                            }
                            else
                            {
                                if (token.IsCancellationRequested)
                                    throw new OperationCanceledException();

                                ensureEnd = 0;
                                //регистрируем параметры!
                            }
                        }
                        await Task.Delay(10);
                    }
                });
            }
            catch (OperationCanceledException ex)
            {
                DpParam.ValueUpdated -= ActualTqTnLen_ValueUpdated;
                logger.Info("JointRecord. Запись прервана по таймауту.");
            }
        }

        DateTime RecordingBeginTimeStamp;
        private void ActualTqTnLen_ValueUpdated(object sender, OperationalParam e)
        {            
            JointResult.Series.Add(
                new TqTnLenPoint()
                {                   
                    Torque = e.Torque,
                    Length = e.Length,
                    Turns = e.Turns,
                    TimeStamp = Convert.ToInt32((DateTime.Now.Subtract(RecordingBeginTimeStamp)).TotalMilliseconds)
                });
        }

        JointResult JointResult;

        //Оценка оператором
        public void Evaluate(uint result)
        {
            Evaluated?.Invoke(this, result);
        }
        private event EventHandler<uint> Evaluated;

        //РЕЗУЛЬТАТ

        JointResult GetResult()
        {
            JointResult.FinalTorque = Dp_ERG_CAM.Value.PMR_MR_MAKEUP_FIN_TQ;
            JointResult.FinalLength = Dp_ERG_CAM.Value.PMR_MR_MAKEUP_LEN;
            JointResult.FinalJVal = Dp_ERG_CAM.Value.PMR_MR_TOTAL_MAKEUP_VAL;
            JointResult.FinalTurns = Dp_ERG_CAM.Value.PMR_MR_MAKEUP_FIN_TN;

            return JointResult;           
        }
    }
    
}
