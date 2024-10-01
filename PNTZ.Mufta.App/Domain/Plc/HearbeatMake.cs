using System;
using System.Threading;
using System.Threading.Tasks;

using DpConnect.Interface;
using Toolkit.IO;

namespace PNTZ.Mufta.App.Domain.Plc
{
    public class HeartbeatMake : IDpProcessor
    {
        public HeartbeatMake(ICliProgram cli)
        {
            _cli = cli;
            SetCliCommands(_cli);
        }
        public string Name { get; set; }

        CancellationTokenSource cts;
        readonly ICliProgram _cli;
        bool running = false;
        void SetCliCommands(ICliProgram cli)
        {
            cli.RegisterCommand("starthb", (args) => StartHeartbeat());
            cli.RegisterCommand("stophb", (args) => StopHeartbeat());
        }

        async void StartHeartbeat()
        {
            if (running)
            {
                _cli.WriteLine($"{Name} уже запущен!");
                return;
            }
            _cli.WriteLine($"{Name} запуск...");
            cts = new CancellationTokenSource();

            try
            {
                await Task.Run(() =>
                {

                    while (true)
                    {
                        if (cts.Token.IsCancellationRequested)
                        {
                            cts.Token.ThrowIfCancellationRequested();
                        }
                        AppHeartbeat.Value = !AppHeartbeat.Value;

                        if (!running)
                        {
                            _cli.WriteLine($"{Name} запущен");
                            running = true;
                        }
                        Thread.Sleep(1000);

                    }
                }, cts.Token);
            }
            catch (OperationCanceledException)
            {
                _cli.WriteLine($"{Name} остановлен");
            }
            catch (Exception e)
            {

                if (!running) _cli.WriteLine($"Не удалось запустить {Name} по причине: {e.Message}");
                else _cli.WriteLine($"Ошибка во время выполнения {Name} по причине: {e.Message}");
                running = false;
            }
            finally
            {
                running = false;
            }

            //try
            //{
            //    await hbTask;
            //}
            //catch (OperationCanceledException)
            //{
            //    _cli.WriteLine($"{Name} остановлен");
            //}
            //catch (Exception ex)
            //{

            //    _cli.WriteLine($"Исключение в хб: {ex.Message}");
            //}
            //finally
            //{
            //    cts?.Dispose();
            //}
        }
        void StopHeartbeat() => cts?.Cancel();

        public void OnDpInitialized()
        {

        }
        public IDpValue<bool> AppHeartbeat { get; set; }
    }
}
