using Promatis.DataPoint.Interface;
using System.Threading.Channels;
using PNTZ.Mufta.Data;

namespace PNTZ.Mufta.RecipeHandling
{
    public class RecipeLoader : ILoadRecipe, IDpProcessor
    {
        public string Name { get; set; } = "Cam1RecipeLoader";

        public void LoadRecipe(ConnectionRecipe recipe)
        {
            DpConRecipe.Value = recipe;
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
