using DpConnect.Interface;
using System;
using System.Runtime.InteropServices;
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
                    Console.WriteLine("Обновился момент!");
                };

                ActualTqTnLen.ValueUpdated += (se, v) =>
                {                    
                    AppInstance.ActualTqTnLen = v;
                };
            };

            AppInstance.AppCli.RegisterCommand("startreg", async (arg) =>
            {
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
            await Task.Run(() =>
            {
                bool started = false;
                while (true)
                {
                    if(!started)
                    {
                        if(ActualTqTnLen.Value.Torque > 1)
                        {
                            started = true;
                            AppInstance.AppLogger.Info("Регистрация параметров начата!");
                            RecordingOperationParamBegun?.Invoke(null, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        if(ActualTqTnLen.Value.Torque < 1)
                        {
                            AppInstance.AppLogger.Info("Регистрация параметров закончена. Готовим результаты");
                            RecordingOperationParamFinished?.Invoke(null, EventArgs.Empty);
                            break;
                        }
                        else
                        {
                            //регистрируем параметры!
                        }
                    }
                    Task.Delay(10).Wait();
                }
            });
        }

        public event EventHandler RecordingOperationParamBegun;

        public event EventHandler RecordingOperationParamFinished;           


        public IDpValue<JointResult> ObservingJointResult {  get; set; }


        public IDpValue<TqTnLen> ActualTqTnLen { get; set; }

        public IDpValue<uint> SetJointCommand { get; set; }

        public IDpValue<uint> CommandFeedback { get; set; }
    }
}
