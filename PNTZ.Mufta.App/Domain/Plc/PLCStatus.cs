using Toolkit.IO;

namespace PNTZ.Mufta.App.Domain.Plc
{
    public class PLCStatus
    {
        ICliProgram _cli;
        public PLCStatus(ICliProgram cli)
        {
            _cli = cli;
            _cli.RegisterCommand("testplc", (_) => TestPlc());
        }

        void TestPlc()
        {
            _cli.WriteLine(ToString());
        }
        public string ToString()
        {
            return "Всё супер!";
        }
    }
}
