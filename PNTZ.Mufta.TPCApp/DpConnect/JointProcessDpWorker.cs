using DpConnect;

using PNTZ.Mufta.TPCApp.Toolbox.Smoothing;

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
using LinqToDB.Tools;

namespace PNTZ.Mufta.TPCApp.DpConnect
{
    public class JointProcessDpWorker : IDpWorker, IJointProcessWorker
    {
        public JointProcessDpWorker(ILogger logger)
        {
            this.logger = logger;
        }

        ILogger logger;

        //Точки Dp - привязаны к OpcUa
        public IDpValue<uint> DpTpcCommand { get; set; }
        public IDpValue<uint> DpPlcCommand { get; set; }
        public IDpValue<OperationalParam> DpParam { get; set; }

        /// <summary>
        /// в метрах
        /// </summary>
        private float LengthOffset { get; set; } = 0;

        public float TorqueSmoothed { get; set; }

        public IDpValue<ERG_CAM> Dp_ERG_CAM { get; set; }
        public IDpValue<ERG_Muffe> Dp_ERG_Muffe { get; set; }
        public IDpValue<ERG_MVS> Dp_ERG_MVS { get; set; }

        public IDpValue<uint> Dp_ERG_CAM_ResultTotal { get; set; }
        
        public void DpBound()
        {
            MovingAverage torqMA = new MovingAverage(3);

            DpParam.ValueUpdated += (s, v) =>
            {
                TorqueSmoothed = (float)torqMA.SmoothValue(v.Torque);
            };
            DpParam.ValueUpdated += SetLastPoint;
        }        

        public TimeSpan CommandAwaitTimeout { get; set; } = TimeSpan.FromSeconds(60);
        public TimeSpan RecordingTimeout { get; set; } = TimeSpan.FromSeconds(60);

        //События соединения

        //Труба появилась на станке. 
        public event EventHandler<JointResult> PipeAppear;
        //Труба в навёрточной головке. Началось свинчивания
        public event EventHandler<EventArgs> RecordingBegun;
        public event EventHandler<JointResult> RecordingFinished;
        //Ожидание оценки оператором
        public event EventHandler AwaitForEvaluation;       
        //Свинчивание завершено
        public event EventHandler<JointResult> JointFinished;     
        //Новая точках данных

        public event EventHandler<TqTnLenPoint> NewTqTnLenPoint;

        private TqTnLenPoint lastPoint;
        public void SetLastPoint(object sender, OperationalParam e)
        {
            TqTnLenPoint point = new TqTnLenPoint()
            {
                Torque = e.Torque,
                Length = e.Length,
                Turns = e.Turns,
                TurnsPerMinute = e.TurnsPerMinute,
                TimeStamp = 0,
            };
            if (recordingBeginTimeStamp > DateTime.MinValue)
            {
                point.TimeStamp = Convert.ToInt32((DateTime.Now.Subtract(recordingBeginTimeStamp)).TotalMilliseconds);

                if (jointResult != null)
                {
                    point.Length = point.Length - LengthOffset + jointResult.MVS_Len;
                }
            }

            //point.TurnsPerMinute = (float)TqTnLenPoint.CalculateTurnsPerMinute(lastPoint, point);


            NewTqTnLenPoint?.Invoke(this, point);
            lastPoint = point;
        }

        private JointRecipe actualRecipe = null;
        public void SetActualRecipe(JointRecipe recipe)
        {
            if (recipe == null)
            {
                logger.Info("[JointProcessDpWorker] Рецепт - null");
                return;
            }

            actualRecipe = recipe;
            RecordingTimeout = TimeSpan.FromSeconds(recipe.MU_Moni_Time);
        }

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
                }
                catch (OperationCanceledException)
                {
                    logger.Info("Прослушивание операции соединения отменено");
                    cyclicallyListen = false;
                }
                catch (InvalidOperationException ex)
                {
                    logger.Error(ex.Message);
                    cyclicallyListen = false;
                    logger.Error("Прослушивание операции соединения возобновится после новой команды от ПЛК.");
                    DpPlcCommand.ValueUpdated += StartOnCommandUpdate;
                }
                catch (InvalidProgramException)
                {
                    logger.Error("Joint. Операция прервана ПЛК. Запускаем еще раз");                    
                }
                catch (TimeoutException ex)
                {
                    logger.Error(ex.Message);
                    cyclicallyListen = false;
                    logger.Info("Прослушивание операции соединения возобновится после новой команды от ПЛК.");
                    DpPlcCommand.ValueUpdated += StartOnCommandUpdate;
                }
                catch (Exception ex)
                {
                    logger.Error("Незивестная ошибка записи операции соединения");
                    logger.Error(ex.Message);
                    cyclicallyListen = false;
                }
                finally
                {
                    if (DpTpcCommand.IsConnected)
                        DpTpcCommand.Value = 0;
                    try
                    {
                        if (jointResult != null)
                        {
                            JointFinished?.Invoke(this, FillResult(jointResult));
                        }
                        else
                            JointFinished?.Invoke(this, FillResult(new JointResult(actualRecipe)));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }
                    logger.Info("Цикл записи завершен.");
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
                    
                    jointResult = new JointResult(actualRecipe)
                    {
                        StartTimeStamp = DateTime.Now,
                        MVS_Len = Dp_ERG_MVS.Value.PMR_Pre_MAKEUP_LEN
                    };
                    logger.Info($"Длина преднавёртки: {jointResult.MVS_Len} м.");
                    PipeAppear?.Invoke(this, jointResult);

                    break;
                }
                else if (awaitCommandFeedback.Task.Result == 0)
                {
                    //Добавим здесь задежрку перед выходов в новый цикл.
                    //Без нее пораждаются множественные подписки. Причина пока не выявлена
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
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

            if (AwaitFor30.Task.Result != 30 && AwaitFor30.Task.Result != 28 
                && AwaitFor30.Task.Result != 25)
            {
                if (AwaitFor30.Task.Result == 0)
                    throw new InvalidProgramException();
                else
                    throw new InvalidOperationException($"Неверный ответ от ПЛК: Ожидалось 30(Навинчивание) или 25(Развинчивание) или 28(Ошибка преднавёртки)");

            }

            //if (AwaitFor30.Task.Result == 28)
            //{
            //    logger.Info("Joint. Ошибка преднавёртки!");
            //    throw new InvalidOperationException("Ошибка преднавёртки");
            //}




            //Труба в позиции. Запускаем таск записи параметров

            logger.Info("Joint. Труба в позиции головки свинчивания. Готовимся к записи параметров! Начальная точка: " + DpParam.Value.Length);

            LengthOffset = DpParam.Value.Length;

            CancellationTokenSource recordCtc = new CancellationTokenSource();
            //Отмена по таймауту. Предполагаю что monitoring time.
            timeout = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(60));
                recordCtc?.Cancel();
            });

            //Устанавливаем 38 - начинаем записывать параметры
            DpTpcCommand.Value = 38;
            
            //Ожидаем 40 - окончание свинчивания
            var AwaitFor40 = new TaskCompletionSource<uint>();
            DpPlcCommand.ValueUpdated += (s, v) => AwaitFor40.TrySetResult(v);

            var recordTask = RecordOperationParams(recordCtc.Token);

            first = await Task.WhenAny(recordTask, timeout, tcs.Task, AwaitFor40.Task);

            if (first == timeout || first == recordTask)
            {
                logger.Info("Таймаут");
                recordCtc.Cancel();
                RecordingFinished?.Invoke(this, jointResult);
                throw new TimeoutException("Время записи параметров истекло");
            }
            else if (first == tcs.Task)
            {
                logger.Info("Отменено");
                recordCtc?.Cancel();
                RecordingFinished?.Invoke(this, jointResult);
                throw new OperationCanceledException();
            }
            else if(first == AwaitFor40.Task) //Свинчивание завершено
            {
                logger.Info("Joint. команда ПЛК:" + AwaitFor40.Task.Result);
                recordCtc?.Cancel();


                if (AwaitFor40.Task.Result != 40)
                {
                    RecordingFinished?.Invoke(this, jointResult);
                    if (AwaitFor40.Task.Result == 0)
                        throw new InvalidProgramException();
                    else
                        throw new InvalidOperationException($"Неверный ответ от ПЛК. Ожидалось 40");
                }
                else
                {

                    await Task.Delay(TimeSpan.FromSeconds(2));

                    jointResult.FinalTorque = Dp_ERG_CAM.Value.PMR_MR_MAKEUP_FIN_TQ;
                    jointResult.FinalLength = Dp_ERG_CAM.Value.PMR_MR_MAKEUP_LEN + jointResult.MVS_Len;
                    jointResult.FinalTurns = Dp_ERG_CAM.Value.PMR_MR_MAKEUP_FIN_TN;

                    logger.Info($"Свинчивание завершено. Итоговый момент: {jointResult.FinalTorque}, итоговая длина: {jointResult.FinalLength}, итоговые обороты: {jointResult.FinalTurns}");
                    logger.Info("Результат ПЛК: " + Dp_ERG_CAM.Value.PMR_MR_MAKEUP_RESULT);

                    RecordingFinished?.Invoke(this, jointResult);
                }

            }
            else
            {
                RecordingFinished?.Invoke(this, jointResult);
                logger.Info($"Неожиданный поворот: {first}");
            }

            //Устанавливаем 45 - ожидание оценки
            DpTpcCommand.Value = 45;                        

            //Оценка. Если годная - то автооценка. Если брак, то подтверждение оператора
            var evaluator = new JointEvaluation(logger);

            if (!evaluator.Evaluate(jointResult))
            {
                TaskCompletionSource<uint> awaitEvaluation = new TaskCompletionSource<uint>();
                Evaluated += (s, v) => awaitEvaluation.TrySetResult(v);
                AwaitForEvaluation?.Invoke(null, EventArgs.Empty);

                var firstTask = await Task.WhenAny(awaitEvaluation.Task, tcs.Task);
                jointResult.ResultTotal = awaitEvaluation.Task.Result;
                logger.Info("Оценка установлена оператором: " + awaitEvaluation.Task.Result);
            }


            //Устанавливаем 50 - отправили оценку
            Dp_ERG_CAM_ResultTotal.Value = jointResult.ResultTotal;

            await Task.Delay(TimeSpan.FromSeconds(2));

            DpTpcCommand.Value = 50;


            //Ожидаем завершения процедуры
            timeout = Task.Delay(CommandAwaitTimeout);
            var awaitFor0 = new TaskCompletionSource<uint>();
            DpPlcCommand.ValueUpdated += (s, v) => awaitFor0.TrySetResult(v);
            

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

            jointResult.FinishTimeStamp = DateTime.Now;

            LengthOffset = 0;
        }

        async Task RecordOperationParams(CancellationToken token)
        {
            logger.Info("Регистрация параметров начата!");
            recordingBeginTimeStamp = DateTime.Now;
            jointResult.Series = new List<TqTnLenPoint>();
            NewTqTnLenPoint += ActualTqTnLen_ValueUpdated;
            RecordingBegun?.Invoke(this, EventArgs.Empty);

            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException)
            {
                NewTqTnLenPoint -= ActualTqTnLen_ValueUpdated;
                logger.Info("JointRecord. Запись параметров остановлена.");
                recordingBeginTimeStamp = DateTime.MinValue;
            }
        }

        DateTime recordingBeginTimeStamp = DateTime.MinValue;
        private void ActualTqTnLen_ValueUpdated(object sender, TqTnLenPoint e)
        {
            jointResult.Series.Add(e);            
        }

        public JointResult jointResult { get; private set; }

        //Оценка оператором
        public void Evaluate(uint result)
        {
            Evaluated?.Invoke(this, result);
           
        }
        private event EventHandler<uint> Evaluated;

        //РЕЗУЛЬТАТ

        JointResult FillResult(JointResult result)
        {
            result.FinalTorque = Dp_ERG_CAM.Value.PMR_MR_MAKEUP_FIN_TQ;
            result.FinalLength = Dp_ERG_CAM.Value.PMR_MR_MAKEUP_LEN;
            result.FinalJVal = Dp_ERG_CAM.Value.PMR_MR_TOTAL_MAKEUP_VAL;
            result.FinalTurns = Dp_ERG_CAM.Value.PMR_MR_MAKEUP_FIN_TN;

            return result;           
        }
    }
    
}
