using System;
using System.ComponentModel;

using static PNTZ.Mufta.TPCApp.App;


namespace PNTZ.Mufta.TPCApp.Domain
{

    public class JointRecipe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Guid _id; 
        public Guid Id { get => _id; set { _id = value; OnPropertyChanged(nameof(Id)); } }

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }

        // Общие данные
        private float _head_close_pulses;
        public float HEAD_OPEN_PULSES { get => _head_close_pulses; set { _head_close_pulses = value; OnPropertyChanged(nameof(HEAD_OPEN_PULSES)); } }

        private float _turnsBrake;
        public float TURNS_BREAK { get => _turnsBrake; set { _turnsBrake = value; OnPropertyChanged(nameof(TURNS_BREAK)); } }

        private ushort _plcProgNr;
        public ushort PLC_PROG_NR { get => _plcProgNr; set { _plcProgNr = value; OnPropertyChanged(nameof(PLC_PROG_NR)); } }

        private ushort _logNo;
        public ushort LOG_NO { get => _logNo; set { _logNo = value; OnPropertyChanged(nameof(LOG_NO)); } }

        private ushort _tqUnit;
        public ushort Tq_UNIT { get => _tqUnit; set { _tqUnit = value; OnPropertyChanged(nameof(Tq_UNIT)); } }
        public ushort Thread_type
        {
            get
            {
                switch(SelectedThreadType)
                {
                    case ThreadType.RIGHT:
                        return 1;
                    case ThreadType.LEFT:
                        return 2;
                    default:
                        return 1;
                }    
            }
        }


        private ThreadType _selectedThreadType;
        public ThreadType SelectedThreadType { get => _selectedThreadType; set { _selectedThreadType = value; OnPropertyChanged(nameof(SelectedThreadType)); } }

        private float _threadStep;
        public float Thread_step { get => _threadStep; set { _threadStep = value; OnPropertyChanged(nameof(Thread_step)); } }

        private string _pipeType;
        public string PIPE_TYPE { get => _pipeType; set { _pipeType = value; OnPropertyChanged(nameof(PIPE_TYPE)); } }

        // Параметры муфты
        private int _boxMoniTime;
        public int Box_Moni_Time { get => _boxMoniTime; set { _boxMoniTime = value; OnPropertyChanged(nameof(Box_Moni_Time)); } }

        private float _boxLenMin;
        public float Box_Len_Min { get => _boxLenMin; set { _boxLenMin = value; OnPropertyChanged(nameof(Box_Len_Min)); } }

        private float _boxLenMax;
        public float Box_Len_Max { get => _boxLenMax; set { _boxLenMax = value; OnPropertyChanged(nameof(Box_Len_Max)); } }

        //Параметры преднавёртки
        private int _preMoniTime;
        public int Pre_Moni_Time { get => _preMoniTime; set { _preMoniTime = value; OnPropertyChanged(nameof(Pre_Moni_Time)); } }

        private float _preLenMax;
        public float Pre_Len_Max { get => _preLenMax; set { _preLenMax = value; OnPropertyChanged(nameof(Pre_Len_Max)); } }

        private float _preLenMin;
        public float Pre_Len_Min { get => _preLenMin; set { _preLenMin = value; OnPropertyChanged(nameof(Pre_Len_Min)); } }

        //Параметры силового свинчивания общие
        private int _muMoniTime;
        public int MU_Moni_Time { get => _muMoniTime; set { _muMoniTime = value; OnPropertyChanged(nameof(MU_Moni_Time)); } }

        private float _muTqRef;
        public float MU_Tq_Ref { get => _muTqRef; set { _muTqRef = value; OnPropertyChanged(nameof(MU_Tq_Ref)); } }

        private float _muTqSave;
        public float MU_Tq_Save { get => _muTqSave; set { _muTqSave = value; OnPropertyChanged(nameof(MU_Tq_Save)); } }       
        public ushort MU_Makeup_Mode // 0 - по моменту, 1 - по длине, 2 - по JVal
        {
            get
            {
                switch(JointMode)
                {
                    case JointMode.Torque:
                    case JointMode.TorqueShoulder:
                        return 0;

                    case JointMode.Length:
                    case JointMode.TorqueLength:
                        return 1;

                    case JointMode.Jval:
                    case JointMode.TorqueJVal:
                        return 2;

                    default:
                        return 0;
                }    
            }
        }
        private JointMode _jointMode;
        public JointMode JointMode { get => _jointMode; set { _jointMode = value; OnPropertyChanged(nameof(JointMode)); } }

        private float _muTqSpeedRed1;
        public float MU_TqSpeedRed_1 { get => _muTqSpeedRed1; set { _muTqSpeedRed1 = value; OnPropertyChanged(nameof(MU_TqSpeedRed_1)); } }

        private float _muTqSpeedRed2;
        public float MU_TqSpeedRed_2 { get => _muTqSpeedRed2; set { _muTqSpeedRed2 = value; OnPropertyChanged(nameof(MU_TqSpeedRed_2)); } }

        private float _muTqDump;
        public float MU_Tq_Dump { get => _muTqDump; set { _muTqDump = value; OnPropertyChanged(nameof(MU_Tq_Dump)); } }

        private float _muTqMax;
        public float MU_Tq_Max { get => _muTqMax; set { _muTqMax = value; OnPropertyChanged(nameof(MU_Tq_Max)); } }

        private float _muTqMin;
        public float MU_Tq_Min { get => _muTqMin; set { _muTqMin = value; OnPropertyChanged(nameof(MU_Tq_Min)); } }

        private float _muTqOpt;
        public float MU_Tq_Opt { get => _muTqOpt; set { _muTqOpt = value; OnPropertyChanged(nameof(MU_Tq_Opt)); } }

        private float _muTqShoulderMin;
        public float MU_TqShoulder_Min { get => _muTqShoulderMin; set { _muTqShoulderMin = value; OnPropertyChanged(nameof(MU_TqShoulder_Min)); } }

        private float _muTqShoulderMax;
        public float MU_TqShoulder_Max { get => _muTqShoulderMax; set { _muTqShoulderMax = value; OnPropertyChanged(nameof(MU_TqShoulder_Max)); } }

        //Параметры силового свинчивания по длине
        private float _muLenSpeed1;
        public float MU_Len_Speed_1 { get => _muLenSpeed1; set { _muLenSpeed1 = value; OnPropertyChanged(nameof(MU_Len_Speed_1)); } }

        private float _muLenSpeed2;
        public float MU_Len_Speed_2 { get => _muLenSpeed2; set { _muLenSpeed2 = value; OnPropertyChanged(nameof(MU_Len_Speed_2)); } }

        private float _muLenDump;
        public float MU_Len_Dump { get => _muLenDump; set { _muLenDump = value; OnPropertyChanged(nameof(MU_Len_Dump)); } }

        private float _muLenMin;
        public float MU_Len_Min { get => _muLenMin; set { _muLenMin = value; OnPropertyChanged(nameof(MU_Len_Min)); } }

        private float _muLenMax;
        public float MU_Len_Max { get => _muLenMax; set { _muLenMax = value; OnPropertyChanged(nameof(MU_Len_Max)); } }

        //Параметры силового свинчивания по J
        private float _muJValSpeed1;
        public float MU_JVal_Speed_1 { get => _muJValSpeed1; set { _muJValSpeed1 = value; OnPropertyChanged(nameof(MU_JVal_Speed_1)); } }

        private float _muJValSpeed2;
        public float MU_JVal_Speed_2 { get => _muJValSpeed2; set { _muJValSpeed2 = value; OnPropertyChanged(nameof(MU_JVal_Speed_2)); } }

        private float _muJValDump;
        public float MU_JVal_Dump { get => _muJValDump; set { _muJValDump = value; OnPropertyChanged(nameof(MU_JVal_Dump)); } }

        private float _muJValMin;
        public float MU_JVal_Min { get => _muJValMin; set { _muJValMin = value; OnPropertyChanged(nameof(MU_JVal_Min)); } }

        private float _muJValMax;
        public float MU_JVal_Max { get => _muJValMax; set { _muJValMax = value; OnPropertyChanged(nameof(MU_JVal_Max)); } }

        private DateTime _timeStamp;
        public DateTime TimeStamp { get => _timeStamp; set { _timeStamp = value; OnPropertyChanged(nameof(TimeStamp)); } }
    }    
}
