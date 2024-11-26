using System;
using System.Threading;
using System.Threading.Tasks;

using DpConnect;

using Promatis.Core.Logging;
using Toolkit.IO;


namespace PNTZ.Mufta.TPCApp.DpConnect
{

    public class MakeHeartBeat : IDpWorker
    {
        ILogger logger;
        CancellationTokenSource cts;
        bool running = false;
        private bool CheckProcedureStarted = false;

        public MakeHeartBeat(ILogger logger)
        {
            this.logger = logger;
        }

        public IDpValue<bool> DpHeartbeat { get; set; }
        public string status { get; set; }        

        public void DpBound()
        {
            DpHeartbeat.StatusChanged += DpHeartbeat_StatusChanged;
         
        }

        private void DpHeartbeat_StatusChanged(object sender, EventArgs e)
        {
            if(DpHeartbeat.IsConnected)
            {
                if(!CheckProcedureStarted)
                    StartHeartbeat();
            }
            else
            {
                cts?.Cancel();
            }
        }

        async void StartHeartbeat()
        {
            if (CheckProcedureStarted)
                throw new InvalidOperationException();

            logger.Info("Запускаем heartbeat...");

            cts = new CancellationTokenSource();
            try
            {
                await Task.Run(async () =>
                {
                    CheckProcedureStarted = true;
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
                            status = "Работает";
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
                CheckProcedureStarted = false;
            }
        }
    }
}
