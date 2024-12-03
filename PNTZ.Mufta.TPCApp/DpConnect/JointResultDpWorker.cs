
using DpConnect;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect.Struct;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
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
            DpPlcCommand.StatusChanged += StopOnDisconnect;
        }



        //События соединения

        //Труба появилась на станке. 
        public event EventHandler<EventArgs> PipeAppear;
        //Труба в навёрточной головке. Началось свинчивания
        public event EventHandler<EventArgs> RecordingBegun;
        public event EventHandler<EventArgs> RecordingFinished;
        //Ожидание оценки оператором
        public event EventHandler AwaitForEvaluation;       
        //Свинчивание завершено
        public event EventHandler<EventArgs> JointFinished;        
        

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
                        if (DpTpcCommand.IsConnected)
                        {
                            Task.Run(() => StartProcedure());
                        }
                        else
                        {
                            DpTpcCommand.StatusChanged += StartOnConnect;
                        }
                    }
                }
            }
        }

        //При пропаже соединения останавливаем прослушку. Если процедура уже начата, то не будет эффекта, но она отвалится по таймауту
        private void StopOnDisconnect(object sender, EventArgs e)
        {
            if (!DpTpcCommand.IsConnected && CyclicallyListen)
            {
                StopAwaiting();
                logger.Info("Прослушивание операции соединения будет возобновлено при подключении");
                DpTpcCommand.StatusChanged += StartOnConnect;
            }

        }

        //При появлении соединения начинаем прослушивать
        private async void StartOnConnect(object sender, EventArgs e)
        {
            if (DpTpcCommand.IsConnected)
            {
                DpTpcCommand.StatusChanged -= StartOnConnect;
                await StartProcedure();
            }
        }

        //Начинаем прослушивать, если изменилась команда от ПЛК
        private async void StartOnCommandUpdate(object sender, uint value)
        {
            logger.Info("Получена команда от ПЛК для параметров машин: " + value);
            DpTpcCommand.ValueUpdated -= StartOnCommandUpdate;
            await StartProcedure();
        }

        //Вход в таск прослушивания
        private async Task StartProcedure()
        {
            CyclicallyListen = true;

            if (JointProcedureStarted)
                throw new InvalidOperationException("Прослушивание операции соединения уже активировано!");

            logger.Info("Активировано Прослушивание операции соединения параметров машин. Ожидаем ПЛК!");

            while (CyclicallyListen)
            {
                try
                {
                    DpTpcCommand.Value = 0;
                    cts = new CancellationTokenSource();
                    JointProcedureStarted = true;
                    await AwaitForJointProcess(cts.Token);
                    logger.Info("Соединение записано.");
                }
                catch (OperationCanceledException ex)
                {
                    logger.Info("Прослушивание операции соединения отменено");
                }
                catch (InvalidOperationException ex)
                {
                    logger.Info(ex.Message);
                    CyclicallyListen = false;
                    logger.Info("Прослушивание операции соединения возобновитс после новой команды от ПЛК.");
                    DpTpcCommand.ValueUpdated += StartOnCommandUpdate;
                }
                catch (Exception ex)
                {
                    logger.Info("Незивестная ошибка записи операции соединения");
                    logger.Info(ex.Message);
                    CyclicallyListen = false;
                }
                finally
                {
                    if (DpTpcCommand.IsConnected)
                        DpTpcCommand.Value = 0;

                    JointFinished?.Invoke(this, EventArgs.Empty);
                }
            }
            logger.Info("Прослушивание операции соединения завершено");
            JointProcedureStarted = false;
        }

        //Отменить ожидание команды от ПЛК для нового соединения
        private void StopAwaiting()
        {
            CyclicallyListen = false;
            DpPlcCommand.ValueUpdated -= StartOnCommandUpdate;
            DpTpcCommand.StatusChanged -= StartOnConnect;
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
            TaskCompletionSource<uint> awaitCommandFeedback = new TaskCompletionSource<uint>();
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            token.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            DpPlcCommand.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            var first = await Task.WhenAny(awaitCommandFeedback.Task, tcs.Task);

            if (first == tcs.Task)
            {
                throw new OperationCanceledException();
            }




            //Команда 10 - свинчивание начинается. Труба подводится к навёрточной головке

            logger.Info("Joint. команда ПЛК:" + awaitCommandFeedback.Task.Result);

            if (awaitCommandFeedback.Task.Result != 10)
                throw new InvalidOperationException("Неверный ответ от ПЛК. Ожидалось 10");
            PipeAppear?.Invoke(this, EventArgs.Empty);
            JointResult = new JointResult()
            {
                StartTimeStamp = DateTime.Now,
            };



            //Отправляем 20 и ждем 30 или 28 - Либо успешно, либо ошибка преднавёртки
            DpTpcCommand.Value = 20;

            var timeout = Task.Delay(TimeSpan.FromSeconds(10));
            awaitCommandFeedback = new TaskCompletionSource<uint>();

            DpPlcCommand.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            first = await Task.WhenAny(awaitCommandFeedback.Task, timeout);

            if (first == timeout)
                throw new TimeoutException("Время ожидания команды истекло");

            logger.Info("Joint. команда ПЛК:" + awaitCommandFeedback.Task.Result);

            if (awaitCommandFeedback.Task.Result != 30 && awaitCommandFeedback.Task.Result != 28)
            {
                throw new InvalidOperationException($"Неверный ответ от ПЛК: {awaitCommandFeedback.Task.Result}");
            }
            if (awaitCommandFeedback.Task.Result == 28)
            {
                logger.Info("Joint. Ошибка преднавёртки!");
            }




            //Труба в позиции. Запускаем таск записи параметров

            logger.Info("Joint. Труба в позиции свинчивания. Готовимся к записи параметров!");

            CancellationTokenSource recordCtc = new CancellationTokenSource();
            //Отмена по таймауту. Предполагаю что monitoring time.
            timeout = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(60));
                recordCtc.Cancel();
            });

            var recordTask = RecordOperationParams(recordCtc.Token);
            first = await Task.WhenAny(recordTask, timeout);

            RecordingFinished?.Invoke(this, EventArgs.Empty);

            if (first == timeout)
                throw new TimeoutException("Время записи параметров истекло");





            //Запись параметров закончена

            logger.Info("Как будто записали параметры");
            awaitCommandFeedback = new TaskCompletionSource<uint>();
            timeout = Task.Delay(TimeSpan.FromSeconds(10));
            DpPlcCommand.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            //Устанавливаем 38 - ответ записали параметры
            DpTpcCommand.Value = 38;

            first = await Task.WhenAny(awaitCommandFeedback.Task, timeout);

            if (first == timeout)
                throw new TimeoutException("Время ожидания команды истекло");

            logger.Info("Joint. команда ПЛК:" + awaitCommandFeedback.Task.Result);
            if (awaitCommandFeedback.Task.Result != 40)
                throw new InvalidOperationException($"Неверный ответ от ПЛК: {awaitCommandFeedback.Task.Result}");


            logger.Info("Итоговый момент:");
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
            timeout = Task.Delay(TimeSpan.FromSeconds(10));
            awaitCommandFeedback = new TaskCompletionSource<uint>();
            DpPlcCommand.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            //Устанавливаем 50 - отправили оценку
            DpTpcCommand.Value = 50;
            await awaitCommandFeedback.Task;

            if (awaitCommandFeedback.Task.Result != 0)
            {
                throw new Exception($"Неверный ответ от ПЛК: {awaitCommandFeedback.Task.Result}");
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
    }
    
}
