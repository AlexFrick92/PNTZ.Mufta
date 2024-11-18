using DpConnect;

namespace PNTZ.Mufta.TPCApp.DpConnect
{
    public interface IMakeHeartBeat : IDpWorker
    {
        string status { get; set; }
    }
}