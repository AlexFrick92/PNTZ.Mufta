using DpConnect;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.DpConnect
{
    public class HeartbeatCheck : IDpWorker
    {
        public IDpValue<bool> DpPlcHeartbeat {  get; set; }
        public bool Beating { get; set; }
        private bool CheckProcedureStarted = false;

        ILogger logger;
        CancellationTokenSource cts;
        public HeartbeatCheck(ILogger logger)
        {
            this.logger = logger;
        }

        public void DpBound()
        {
            DpPlcHeartbeat.StatusChanged += DpPlcHeartbeat_StatusChanged;
        }

        private void DpPlcHeartbeat_StatusChanged(object sender, EventArgs e)
        {
            if (DpPlcHeartbeat.IsConnected)
            {
                if(!CheckProcedureStarted)
                   StartHeartbeatCheck();            
            }
            else            
                cts?.Cancel();           
        }

        private async void StartHeartbeatCheck() 
        {
            logger.Info("Запускаем проверку heartbeat...");

            cts = new CancellationTokenSource();

            if (CheckProcedureStarted)
                throw new InvalidOperationException("Проверка бита жизни уже запущена");

            CheckProcedureStarted = true;

            await Task.Run(async () =>
            {                
                try
                {
                    while (true)
                    {
                        try
                        {
                            if (cts.Token.IsCancellationRequested)
                            {
                                throw new OperationCanceledException();
                            }
                            var awaitHeartbeat = new TaskCompletionSource<bool>();
                            var timeout = Task.Delay(TimeSpan.FromSeconds(2));
                            DpPlcHeartbeat.ValueUpdated += (s, v) => awaitHeartbeat.TrySetResult(v);

                            var first = await Task.WhenAny(awaitHeartbeat.Task, timeout);

                            if (first == timeout)
                            {
                                throw new TimeoutException("Время heartbeat истекло");
                            }
                            if (!Beating)
                            {
                                Beating = true;
                                logger.Info("Появился бит жизни от ПЛК");
                            }
                        }
                        catch (TimeoutException)
                        {
                            if(Beating)
                            {
                                Beating = false;
                                logger.Info("Пропал Бит жизни от ПЛК");
                            }
                        }                        
                    }
                }
                catch (OperationCanceledException)
                {
                    logger.Info($"heartbeatCheck остановлен");       

                }                
                catch (Exception ex)
                {
                    logger.Error($"Не удалось запустить heartbeatCheck по причине: {ex.Message}");         

                }                
                finally
                {
                    Beating = false;
                }
                
            }, cts.Token);

            CheckProcedureStarted = false;


        }
    }
}
