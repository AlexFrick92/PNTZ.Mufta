using Promatis.DataPoint.Configuration;
using Promatis.DataPoint.Interface;
using Toolkit.IO;

namespace PNTZ.Mufta.App
{
    public class DpArrayReader : DpProcessor
    {
        ICliProgram _cli;
        public DpArrayReader(ICliProgram cli)
        {
            _cli = cli;

            dpArray = new List<IDpValue<short>>();
        }

        IList<IDpValue<short>> dpArray;
        public IDpValue<short> ArrayMember1 
        { 
            set 
            {
                value.ValueUpdated += (s, v) => Console.WriteLine($"Считано: {v}");
                dpArray.Add(value);
            } 
        }
    }
}
