using Promatis.DataPoint.Interface;
using TorqueControl.Data;

namespace Cam
{
    public class RecipeLoader : ILoadRecipe, IDpProcessor
    {
        public string Name { get; set; } = "Cam1RecipeLoader";

        public void LoadRecipe(ConnectionRecipe recipe)
        {
            HEAD_OPEN_PULSES.Value = recipe.HEAD_OPEN_PULSES;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        #region DataPoints
        public IDpValue<float> HEAD_OPEN_PULSES { get; set; }

        #endregion
    }
}
