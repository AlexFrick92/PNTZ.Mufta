using System;
using System.Threading;
using System.Threading.Tasks;

using DpConnect.Interface;
using Toolkit.IO;

namespace PNTZ.Mufta.App.Domain.Plc
{
    public class HeartbeatCheck : IDpProcessor
        {
            readonly ICliProgram _cli;
            TimeSpan _heartbeatInterval = TimeSpan.FromMilliseconds(3000);
            ManualResetEvent newBeat = new ManualResetEvent(false);
            CancellationTokenSource cts;
            public bool Beating { get; set; }
            public HeartbeatCheck(ICliProgram cli)
            {
                _cli = cli;
                _cli.RegisterCommand("starthbc", (args) => StartChecking());
                _cli.RegisterCommand("stophbc", (args) => StopChecking());
            }
            public string Name { get; set; } = "HeartbeatCheck";

            public void OnDpInitialized()
            {
            }
            bool running = false;
            async void StartChecking()
            {
                running = true;
                _cli.WriteLine($"{Name} запущен.");
                PlcHeartbeat.ValueUpdated += PlcHeartbeat_ValueUpdated;
                cts = new CancellationTokenSource();

                try
                {
                    await Task.Run(() =>
                    {
                        while (true)
                        {
                            newBeat.Reset();
                            if (cts.IsCancellationRequested)
                            {
                                cts.Token.ThrowIfCancellationRequested();
                            }
                            else
                            {
                                if (newBeat.WaitOne(_heartbeatInterval))
                                {
                                    if (!Beating)
                                    {
                                        Beating = true;
                                        _cli.WriteLine($"{Name}: Появился хартбит!");
                                    }
                                }
                                else
                                {
                                    if (Beating)
                                    {
                                        Beating = false;
                                        _cli.WriteLine($"{Name}: пропал хартбит!");
                                    }
                                }
                            }
                        }

                    });
                }
                catch (OperationCanceledException ex)
                {

                }
                catch
                {

                }
                finally
                {
                    Beating = false;
                    _cli.WriteLine($"{Name} Остановлен.");
                    PlcHeartbeat.ValueUpdated -= PlcHeartbeat_ValueUpdated;
                }
            }

            private void PlcHeartbeat_ValueUpdated(object sender, bool e)
            {
                newBeat.Set();
            }

            void StopChecking()
            {
                cts?.Cancel();
            }
            public IDpValue<bool> PlcHeartbeat { get; set; }
        }
    
}
    