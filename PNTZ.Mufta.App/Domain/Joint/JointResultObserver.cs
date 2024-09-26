using DevExpress.Charts.Model;
using DpConnect.Interface;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static PNTZ.Mufta.App.App;

namespace PNTZ.Mufta.App.Domain.Joint
{
    public class JointResultObserver : DpProcessor
    {        
        public JointResultObserver()
        {
            DpInitialized += (s, e) =>
            {
                CommandFeedback.ValueUpdated += BeginJointRecording;
                AppInstance.LastJointResult = ObservingJointResult.Value;

                ObservingJointResult.ValueUpdated += (se, v) =>
                {
                    AppInstance.LastJointResult = v;                    
                };

                ActualTqTnLen.ValueUpdated += (se, v) =>
                {                    
                    AppInstance.ActualTqTnLen = v;
                };
            };

            AppInstance.AppCli.RegisterCommand("startreg", async (arg) =>
            {
                AppInstance.AppCli.WriteLine("Активирована запись графика. Ожидаем рост момента!");
                await RecordOperationParams();
            });
        }

        async void BeginJointRecording(object sender, uint command)
        {
            if(command == 10)
            {
                CommandFeedback.ValueUpdated -= BeginJointRecording;
                AppInstance.AppLogger.Info("Запись начата!");

                try
                {
                    await RecordJoint();
                    AppInstance.AppLogger.Info("Запись закончена!");
                }
                catch
                {
                    AppInstance.AppLogger.Info("Запись прервана!");
                }
                finally
                {
                    CommandFeedback.ValueUpdated += BeginJointRecording;
                }

            }
        }

        async Task RecordJoint()
        {
            TaskCompletionSource<uint> awaitCommandFeedback = new TaskCompletionSource<uint>();

            CommandFeedback.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);
            
            SetJointCommand.Value = 20;

            await awaitCommandFeedback.Task;

            if (awaitCommandFeedback.Task.Result != 30)
            {
                throw new Exception();
            }

            AppInstance.AppLogger.Info("Труба в позиции свинчивания. Готовимся к записи параметров!");

            await RecordOperationParams();

            awaitCommandFeedback = new TaskCompletionSource<uint>();

            CommandFeedback.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            SetJointCommand.Value = 38;

            await awaitCommandFeedback.Task;

            if(awaitCommandFeedback.Task.Result != 40)
            {
                throw new Exception();
            }

            AppInstance.AppLogger.Info("Итоговый момент:" + ObservingJointResult.Value.FinalTorque);            

            SetJointCommand.Value = 50;
        }
        
        async Task RecordOperationParams()
        {
            try
            {
                await Task.Run(() =>
                {
                    bool started = false;

                    CancellationTokenSource ctc = new CancellationTokenSource();

                    int ensureEnd = 0;

                    while (true)
                    {
                        if (!started)
                        {
                            if (ActualTqTnLen.Value.Torque > 1)
                            {
                                started = true;
                                AppInstance.AppLogger.Info("Регистрация параметров начата!");
                                RecordingOperationParamBegun?.Invoke(null, EventArgs.Empty);

                                //Отмена по таймауту. Предполагаю что monitoring time.
                                Task.Run(async () =>
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(60));
                                    ctc.Cancel();
                                });
                            }
                        }
                        else
                        {
                            if (ActualTqTnLen.Value.Torque < 1)
                            {
                                if (ensureEnd > 5)
                                {
                                    break;
                                }
                                else
                                    ensureEnd++;
                            }
                            else
                            {
                                if (ctc.Token.IsCancellationRequested)
                                    ctc.Token.ThrowIfCancellationRequested();

                                ensureEnd = 0;
                                //регистрируем параметры!
                            }
                        }
                        Task.Delay(10).Wait();
                    }
                });
            }
            catch (OperationCanceledException ex)
            {
                AppInstance.AppLogger.Info("Запись прервана по таймауту.");
            }
            

            AppInstance.AppLogger.Info("Регистрация параметров закончена. Готовим результаты");
            RecordingOperationParamFinished?.Invoke(null, EventArgs.Empty);
        }

        public event EventHandler RecordingOperationParamBegun;

        public event EventHandler RecordingOperationParamFinished;           


        public IDpValue<JointResult> ObservingJointResult {  get; set; }


        public IDpValue<TqTnLen> ActualTqTnLen { get; set; }

        public IDpValue<uint> SetJointCommand { get; set; }

        public IDpValue<uint> CommandFeedback { get; set; }
    }
}
