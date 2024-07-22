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
        public RecipeLoader(ILogger logger)
        {
            _logger = logger;                            
        }
        public string Name { get; set; } = "Cam1RecipeLoader";

        public void DpInitialized()
        {
            DpConRecipe.ValueChanged += (s, v) => Console.WriteLine(v.TURNS_BREAK + " " + v.HEAD_OPEN_PULSES);
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
            DpConRecipe.Value = recipe;
            _logger.Info($"Рецепт загружен");
        }

        #region DataPoints
        public IDpValue<JointRecipe> DpConRecipe { get; set; }

        public IDpValue<ushort> SetLoadCommand { get; set; }

        public IDpValue<ushort> CommandFeedback { get; set; }       

        #endregion
    }
}
