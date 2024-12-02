using DevExpress.Xpf.Charts;
using DpConnect;
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
    public class HeartbeatCheck : IDpWorker
    {
        public IDpValue<bool> DpPlcHeartbeat {  get; set; }

        bool beating;
        public bool Beating
        {
            get => beating;

            set
            {
                beating = value;
                if (value)
                    HeartBeatApper?.Invoke(this, EventArgs.Empty);
                else
                    HeartBeatDisapper?.Invoke(this, EventArgs.Empty);
            }
        }
        private bool CheckProcedureStarted = false;

        public event EventHandler<EventArgs> HeartBeatApper;
        public event EventHandler<EventArgs> HeartBeatDisapper;
        public event EventHandler<bool> PlcHeartbeat;
        public event EventHandler<bool> PlcStatusChanged;

        ILogger logger;
        CancellationTokenSource cts;
        public HeartbeatCheck(ILogger logger)
        {
            this.logger = logger;
        }

        public void DpBound()
        {
            DpPlcHeartbeat.StatusChanged += DpPlcHeartbeat_StatusChanged;
            DpPlcHeartbeat.ValueUpdated += (s, v) => PlcHeartbeat?.Invoke(s, v);
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
            PlcStatusChanged?.Invoke(this, DpPlcHeartbeat.IsConnected);
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
                            if (Beating)
                            {
                                Beating = false;
                                logger.Info("Пропал Бит жизни от ПЛК");                         
                            }                                
                        }
                        else
                        {
                            if (!Beating)
                            {
                                Beating = true;
                                logger.Info("Появился бит жизни от ПЛК");
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
