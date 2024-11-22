
namespace PNTZ.Mufta.TPCApp.Domain
{
    public enum JointMode
    {
        Torque, //Контроль момента
        TorqueShoulder, //Контроль момента и заплечника
        Length, //Контроль длины
        TorqueLength, //Контроль длины и момента
        Jval, //Контроль значения J
        TorqueJVal //Контроль значения J и момента
    }
}
