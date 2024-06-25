using Promatis.DataPoint.Interface;
using System.Threading.Channels;
using PNTZ.Mufta.Data;
using Promatis.Core.Logging;

namespace PNTZ.Mufta.RecipeHandling
{
    public class RecipeLoader : ILoadRecipe, IDpProcessor
    {
        private readonly ILogger _logger;
        public RecipeLoader(ILogger logger)
        {
            _logger = logger;
        }
        public string Name { get; set; } = "Cam1RecipeLoader";

        public void LoadRecipe(ConnectionRecipe recipe)
        {
            DpConRecipe.Value = recipe;
            _logger.Info($"Рецепт загружен");
        }

        public void DpInitialized()
        {
            DpConRecipe.ValueChanged += (s, v) => Console.WriteLine(v.TURNS_BREAK + " " + v.HEAD_OPEN_PULSES);
        }

        #region DataPoints
        public IDpValue<ConnectionRecipe> DpConRecipe { get; set; }

        public IDpValue<ushort> SetLoadCommand { get; set; }

        public IDpValue<ushort> CommandFeedback { get; set; }

        #endregion
    }
}
