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

            Console.WriteLine(MathOperationGeneric.Call(new TwoNumber() {Number1 = 2.1f, Number2 = 1 }));

            Console.WriteLine(PowerOperation.Call(2));
        }

        #region DataPoints
        public IDpValue<float> BOX_MONI_TIME { get; set; }


        public IDpValue<ConnectionRecipe> DpConRecipe { get; set; }

        public IDpValue<DateTime> CreatingTimeStamp { get; set; }

        public IDpMethod<TwoNumber, float> MathOperationGeneric { get; set; }

        public IDpMethod<float, float> PowerOperation { get; set; }

        #endregion
    }

    public class TwoNumber
    {
        public float Number1 { get; set; }
        public short Number2 { get; set; }
    }
    public class TwoNumberResult
    {
        public float Result { get; set; }
    }
}
