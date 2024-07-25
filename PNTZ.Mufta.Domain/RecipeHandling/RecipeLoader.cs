using Promatis.DataPoint.Interface;
using PNTZ.Mufta.Data;
using Promatis.Core.Logging;
using System.Text.Json;
namespace PNTZ.Mufta.RecipeHandling
{
    /// <summary>
    ///  Класс загружает рецепт в ПЛК. Загрузка рецепта выполняется обменом комманд
    /// </summary>
    public class RecipeLoader : IDpProcessor
    {
        private readonly ILogger _logger;
        private readonly AutoResetEvent recipeLoaded = new AutoResetEvent(false);
        private readonly AutoResetEvent commandAccepted = new AutoResetEvent(false);
        private readonly AutoResetEvent operationCanceled = new AutoResetEvent(false);
        public RecipeLoader(ILogger logger)
        {
            _logger = logger;                            
        }
        public string Name { get; set; } = "Cam1RecipeLoader";

        public void DpInitialized()
        {
            DpJointRecipe.ValueUpdated += (s, v) => Console.WriteLine(v.TURNS_BREAK + " " + v.HEAD_OPEN_PULSES);
        }
        public void LoadRecipe(JointRecipe recipe)
        {
            _logger.Info("Загрузка рецепта...");
            DpJointRecipe.Value = recipe;
            
            _logger.Info($"Рецепт загружен");
        }
        public async void Load()
        {

            _ = Task.Run(() =>
            {
                Task.Delay(10000).Wait();
                operationCanceled.Set();
            });

            await Task.Run(() =>
            {
                try
                {
                    CommandFeedback.ValueUpdated += CommandFeedback_ValueUpdated;
                    _logger.Info("Устанавливаем команду 10");
                    SetLoadCommand.Value = 10;

                    _logger.Info("Ждем ответа 20");

                    WaitHandle[] waitHandles = new WaitHandle[] { commandAccepted, operationCanceled};

                    int index = WaitHandle.WaitAny(waitHandles);

                    if(index == 1)
                    {
                        throw new Exception("Время ожидания истекло");
                    }
                    if (CommandFeedback.Value == 20)
                    {
                        _logger.Info("Устанавливаем команду 30");                       
                        SetLoadCommand.Value = 20;

                        _logger.Info("Ждем ответа 30");
                        commandAccepted.WaitOne();

                        if(CommandFeedback.Value == 30)
                        {
                            _logger.Info("Рецепт загружен!");
                        }
                        else
                        {
                            throw new Exception("Wrong command");
                        }
                    }
                    else
                    {
                        throw new Exception("Wrong command");
                    }
                }
                catch
                {
                    _logger.Info("Загрузить не удалось");
                }
                finally 
                {
                    CommandFeedback.ValueUpdated -= CommandFeedback_ValueUpdated;
                }
            });

        }
        
        private void CommandFeedback_ValueUpdated(object? sender, uint e)
        {
            _logger.Info($"Получена команда: {e}");
            commandAccepted.Set();            
        }



        #region DataPoints
        public IDpValue<JointRecipe> DpJointRecipe { get; set; }

        public IDpValue<uint> SetLoadCommand { get; set; }

        public IDpValue<uint> CommandFeedback { get; set; }       

        #endregion
    }
}
