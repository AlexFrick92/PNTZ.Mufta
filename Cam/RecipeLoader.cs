using Promatis.DataPoint.Interface;
using System.Threading.Channels;
using TorqueControl.Data;

namespace Cam
{
    public class RecipeLoader : ILoadRecipe, IDpProcessor
    {
        public string Name { get; set; } = "Cam1RecipeLoader";

        public void LoadRecipe(ConnectionRecipe recipe)
        {
            BOX_MONI_TIME.Value = recipe.HEAD_OPEN_PULSES;
        }

        public void DpInitialized()
        {
            Console.WriteLine($"{this.Name} : Запускаем подписки");

            DpConRecipe.ValueChanged += (s, v) => Console.WriteLine(v.HEAD_OPEN_PULSES);

            CreatingTimeStamp.ValueChanged += (s, v) => Console.WriteLine(v);

            BOX_MONI_TIME.ValueChanged += (s, v) => Console.WriteLine(v);
            
        }

        #region DataPoints
        public IDpValue<float> BOX_MONI_TIME { get; set; }


        public IDpValue<ConnectionRecipe> DpConRecipe { get; set; }

        public IDpValue<DateTime> CreatingTimeStamp { get; set; }
        


        #endregion
    }
}
