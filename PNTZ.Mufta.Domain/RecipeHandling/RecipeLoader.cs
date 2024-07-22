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
        public RecipeLoader(ILogger logger)
        {
            _logger = logger;                            
        }
        public string Name { get; set; } = "Cam1RecipeLoader";

        public void DpInitialized()
        {
            DpConRecipe.ValueUpdated += (s, v) => Console.WriteLine(v.TURNS_BREAK + " " + v.HEAD_OPEN_PULSES);
        }
        public void LoadRecipe(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                JointRecipe jointRecipe = JsonSerializer.Deserialize<JointRecipe>(fs);
                LoadRecipe(jointRecipe);
            }            
        }
        public void LoadRecipe(JointRecipe recipe)
        {
            _logger.Info("Загрузка рецепта...");
            DpConRecipe.Value = recipe;
            
            _logger.Info($"Рецепт загружен");
        }
        uint awaitedCommand = 0;
        public async void Load()
        {
            await Task.Run(() =>
            {
                _logger.Info("Устанавливаем команду 10");
                SetLoadCommand.Value = 10;
                
                _logger.Info("Ждем ответа");
                awaitedCommand = 20;
                CommandFeedback.ValueUpdated += CommandFeedback_ValueUpdated;                
                commandAccepted.WaitOne();
                CommandFeedback.ValueUpdated -= CommandFeedback_ValueUpdated;

                _logger.Info("Устанавливаем команду 20");
                SetLoadCommand.Value = 20;
            });
            _logger.Info("Рецепт загружен!");
        }
        
        private void CommandFeedback_ValueUpdated(object? sender, uint e)
        {
            if (e == awaitedCommand)
            {
                commandAccepted.Set();
                _logger.Info("Команда получена");
            }
            else
            {
                throw new Exception("Пришла неправильная команда!");
            }

        }



        #region DataPoints
        public IDpValue<JointRecipe> DpConRecipe { get; set; }

        public IDpValue<uint> SetLoadCommand { get; set; }

        public IDpValue<uint> CommandFeedback { get; set; }       

        #endregion
    }
}
