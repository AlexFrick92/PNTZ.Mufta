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
            };
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

            AppInstance.AppLogger.Info("Труба в позиции свинчивания. Начинаем запись параметров!");

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
            ObservingJointResult.ValueUpdated += ObservingJointResult_ValueUpdated;

            TaskCompletionSource<uint> awaitRecording = new TaskCompletionSource<uint>();

            RecordingOperationParamFinished += (s, v) => awaitRecording.TrySetResult(1);

            await awaitRecording.Task;

            ObservingJointResult.ValueUpdated -= ObservingJointResult_ValueUpdated;

        }

        private void ObservingJointResult_ValueUpdated(object sender, JointResult e)
        {
            if(e.ActualTorque < 0)
            {
                AppInstance.AppLogger.Info("Свинчивание завершено. Готовим результаты");
                RecordingOperationParamFinished(null, EventArgs.Empty);
            }
            else
            {
                AppInstance.AppLogger.Info("Текущий момент:" + e.ActualTorque);
            }
        }

        private event EventHandler RecordingOperationParamFinished;           





        public IDpValue<JointResult> ObservingJointResult {  get; set; }

        public IDpValue<uint> SetJointCommand { get; set; }

        public IDpValue<uint> CommandFeedback { get; set; }
    }
}
