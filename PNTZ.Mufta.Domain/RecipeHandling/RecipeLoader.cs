using Promatis.DataPoint.Interface;
using TorqueControl.RecipeHandling;
using PNTZ.Mufta.Data;
using Promatis.Core.Logging;
using TorqueControl.Data;

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

        public void DpInitialized()
        {
            DpConRecipe.ValueChanged += (s, v) => Console.WriteLine(v.TURNS_BREAK + " " + v.HEAD_OPEN_PULSES);
        }

        public void LoadRecipe<T>(ConnectionRecipe<T> recipe) where T : ConnectionRecipe<T>, new()
        {
            DpConRecipe.Value = recipe as PNTZ.Mufta.Data.ConnectionRecipe;
            _logger.Info($"Рецепт загружен");
        }

        #region DataPoints
        public IDpValue<ConnectionRecipe> DpConRecipe { get; set; }

        public IDpValue<ushort> SetLoadCommand { get; set; }

        public IDpValue<ushort> CommandFeedback { get; set; }


        public IDpValue<string> Pipe_type { get; set; }

        #endregion
    }
}
