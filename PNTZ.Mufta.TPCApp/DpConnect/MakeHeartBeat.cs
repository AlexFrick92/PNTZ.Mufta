using System;
using System.Threading;
using System.Threading.Tasks;

using DpConnect;

using Promatis.Core.Logging;


namespace PNTZ.Mufta.TPCApp.DpConnect
{
    public class MakeHeartBeat : IMakeHeartBeat
    {
        ILogger logger;
        CancellationTokenSource cts;
        bool running = false;
        
        
        public MakeHeartBeat(ILogger logger)
        {
            this.logger = logger;
        }

        public IDpValue<bool> DpHeartbeat { get; set; }

        public void DpBound()
        {
            DpHeartbeat.StatusChanged += DpHeartbeat_StatusChanged;
         
        }

        private void DpHeartbeat_StatusChanged(object sender, EventArgs e)
        {
            if(DpHeartbeat.IsConnected)
            {
                StartHeartbeat();
            }
            else
            {
                cts?.Cancel();
            }
        }

        async void StartHeartbeat()
        {
            logger.Info("Запускаем heartbeat...");

            cts = new CancellationTokenSource();
            try
            {
                await Task.Run(async () =>
                {
                    while (true)
                    {
                        if(cts.Token.IsCancellationRequested)
                        {
                            cts.Token.ThrowIfCancellationRequested();
                        }
                        else
                        {
                            DpHeartbeat.Value = !DpHeartbeat.Value;
                        }    
                        if(!running)
                        {
                            logger.Info("heartbeat запущен.");
                            running = true;
                        }
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }, cts.Token);
            }
            catch (OperationCanceledException)
            {
                logger.Info($"heartbeat остановлен");
            }
            catch (Exception e)
            {
                logger.Error($"Не удалось запустить heartbeat по причине: {e.Message}");
            }   
            finally
            {
                running = false;
            }
        }
    }
}
