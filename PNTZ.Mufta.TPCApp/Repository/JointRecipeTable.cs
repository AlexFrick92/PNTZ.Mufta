using LinqToDB;
using LinqToDB.Mapping;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Repository
{
    [Table(Name = "Recipes")]
    public class JointRecipeTable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private Guid _id = Guid.NewGuid();

        [PrimaryKey]
        [Column(DataType = DataType.Guid)]
        public Guid Id { get => _id; set { _id = value; OnPropertyChanged(nameof(Id)); } }



        // Общие данные
        private string _name;
        [Column] public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }
        
        private float _head_close_pulses;
        [Column] public float HEAD_OPEN_PULSES { get => _head_close_pulses; set { _head_close_pulses = value; OnPropertyChanged(nameof(HEAD_OPEN_PULSES)); } }

        private float _turnsBrake;
        [Column] public float TURNS_BREAK { get => _turnsBrake; set { _turnsBrake = value; OnPropertyChanged(nameof(TURNS_BREAK)); } }

        private ushort _plcProgNr;
        [Column] public ushort PLC_PROG_NR { get => _plcProgNr; set { _plcProgNr = value; OnPropertyChanged(nameof(PLC_PROG_NR)); } }

        private ushort _logNo;
        [Column] public ushort LOG_NO { get => _logNo; set { _logNo = value; OnPropertyChanged(nameof(LOG_NO)); } }
        
        private ushort _tqUnit;
        [Column] public ushort Tq_UNIT { get => _tqUnit; set { _tqUnit = value; OnPropertyChanged(nameof(Tq_UNIT)); } }

        //Нужно для ПЛК
        public ushort Thread_type
        {
            get
            {
                switch (ThreadType)
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
        public ThreadType ThreadType { get => (ThreadType)SelectedThreadType; set { SelectedThreadType = (long)value; OnPropertyChanged(nameof(SelectedThreadType)); } }
        [Column] public long SelectedThreadType { get; set; }

        private float _threadStep;
        [Column] public float Thread_step { get => _threadStep; set { _threadStep = value; OnPropertyChanged(nameof(Thread_step)); } }

        private string _pipeType;
        [Column] public string PIPE_TYPE { get => _pipeType; set { _pipeType = value; OnPropertyChanged(nameof(PIPE_TYPE)); } }



        // Параметры муфты
        private int _boxMoniTime;
        [Column] public int Box_Moni_Time { get => _boxMoniTime; set { _boxMoniTime = value; OnPropertyChanged(nameof(Box_Moni_Time)); } }

        private float _boxLenMin;
        [Column] public float Box_Len_Min { get => _boxLenMin; set { _boxLenMin = value; OnPropertyChanged(nameof(Box_Len_Min)); } }

        private float _boxLenMax;
        [Column] public float Box_Len_Max { get => _boxLenMax; set { _boxLenMax = value; OnPropertyChanged(nameof(Box_Len_Max)); } }



        // Параметры предварительной навёртки
        private int _preMoniTime;
        [Column] public int Pre_Moni_Time { get => _preMoniTime; set { _preMoniTime = value; OnPropertyChanged(nameof(Pre_Moni_Time)); } }

        private float _preLenMax;
        [Column] public float Pre_Len_Max { get => _preLenMax; set { _preLenMax = value; OnPropertyChanged(nameof(Pre_Len_Max)); } }

        private float _preLenMin;
        [Column] public float Pre_Len_Min { get => _preLenMin; set { _preLenMin = value; OnPropertyChanged(nameof(Pre_Len_Min)); } }



        // Общие параметры силового свинчивания
        private int _muMoniTime;
        [Column] public int MU_Moni_Time { get => _muMoniTime; set { _muMoniTime = value; OnPropertyChanged(nameof(MU_Moni_Time)); } }

        private float _muTqRef;
        [Column] public float MU_Tq_Ref { get => _muTqRef; set { _muTqRef = value; OnPropertyChanged(nameof(MU_Tq_Ref)); } }

        private float _muTqSave;
        [Column] public float MU_Tq_Save { get => _muTqSave; set { _muTqSave = value; OnPropertyChanged(nameof(MU_Tq_Save)); } }

        //Нужно для ПЛК
        public ushort MU_Makeup_Mode // 0 - по моменту, 1 - по длине, 2 - по JVal
        {
            get
            {
                switch (JointMode)
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
        public JointMode JointMode { get => (JointMode)SelectedMode; set { SelectedMode = (long)value; OnPropertyChanged(nameof(JointMode)); } }
        [Column] public long SelectedMode { get; set; }



        // Параметры силового свинчивания по моменту
        private float _muTqSpeedRed1;
        [Column] public float MU_TqSpeedRed_1 { get => _muTqSpeedRed1; set { _muTqSpeedRed1 = value; OnPropertyChanged(nameof(MU_TqSpeedRed_1)); } }

        private float _muTqSpeedRed2;
        [Column] public float MU_TqSpeedRed_2 { get => _muTqSpeedRed2; set { _muTqSpeedRed2 = value; OnPropertyChanged(nameof(MU_TqSpeedRed_2)); } }

        private float _muTqDump;
        [Column] public float MU_Tq_Dump { get => _muTqDump; set { _muTqDump = value; OnPropertyChanged(nameof(MU_Tq_Dump)); } }

        private float _muTqMax;
        [Column] public float MU_Tq_Max { get => _muTqMax; set { _muTqMax = value; OnPropertyChanged(nameof(MU_Tq_Max)); } }

        private float _muTqMin;
        [Column] public float MU_Tq_Min { get => _muTqMin; set { _muTqMin = value; OnPropertyChanged(nameof(MU_Tq_Min)); } }

        private float _muTqOpt;
        [Column] public float MU_Tq_Opt { get => _muTqOpt; set { _muTqOpt = value; OnPropertyChanged(nameof(MU_Tq_Opt)); } }

        private float _muTqShoulderMin;
        [Column] public float MU_TqShoulder_Min { get => _muTqShoulderMin; set { _muTqShoulderMin = value; OnPropertyChanged(nameof(MU_TqShoulder_Min)); } }

        private float _muTqShoulderMax;
        [Column] public float MU_TqShoulder_Max { get => _muTqShoulderMax; set { _muTqShoulderMax = value; OnPropertyChanged(nameof(MU_TqShoulder_Max)); } }



        // Параметры силового свинчивания по длине
        private float _muLenSpeed1;
        [Column] public float MU_Len_Speed_1 { get => _muLenSpeed1; set { _muLenSpeed1 = value; OnPropertyChanged(nameof(MU_Len_Speed_1)); } }

        private float _muLenSpeed2;
        [Column] public float MU_Len_Speed_2 { get => _muLenSpeed2; set { _muLenSpeed2 = value; OnPropertyChanged(nameof(MU_Len_Speed_2)); } }

        private float _muLenDump;
        [Column] public float MU_Len_Dump { get => _muLenDump; set { _muLenDump = value; OnPropertyChanged(nameof(MU_Len_Dump)); } }

        private float _muLenMin;
        [Column] public float MU_Len_Min { get => _muLenMin; set { _muLenMin = value; OnPropertyChanged(nameof(MU_Len_Min)); } }

        private float _muLenMax;
        [Column] public float MU_Len_Max { get => _muLenMax; set { _muLenMax = value; OnPropertyChanged(nameof(MU_Len_Max)); } }



        // Параметры силового свинчивания по JVal
        private float _muJValSpeed1;
        [Column] public float MU_JVal_Speed_1 { get => _muJValSpeed1; set { _muJValSpeed1 = value; OnPropertyChanged(nameof(MU_JVal_Speed_1)); } }

        private float _muJValSpeed2;
        [Column] public float MU_JVal_Speed_2 { get => _muJValSpeed2; set { _muJValSpeed2 = value; OnPropertyChanged(nameof(MU_JVal_Speed_2)); } }

        private float _muJValDump;
        [Column] public float MU_JVal_Dump { get => _muJValDump; set { _muJValDump = value; OnPropertyChanged(nameof(MU_JVal_Dump)); } }

        private float _muJValMin;
        [Column] public float MU_JVal_Min { get => _muJValMin; set { _muJValMin = value; OnPropertyChanged(nameof(MU_JVal_Min)); } }

        private float _muJValMax;
        [Column] public float MU_JVal_Max { get => _muJValMax; set { _muJValMax = value; OnPropertyChanged(nameof(MU_JVal_Max)); } }

        private DateTime _timeStamp;
        [Column] public DateTime TimeStamp { get => _timeStamp; set { _timeStamp = value; OnPropertyChanged(nameof(TimeStamp)); } }
        
        [Column] public DateTime? RemovedDate { get; set; }

        public JointRecipeTable FromJointRecipe(JointRecipe recipe)
        {
            if (recipe == null) throw new ArgumentNullException("Recipe is Null");

            Name = recipe.Name;
            HEAD_OPEN_PULSES = recipe.HEAD_OPEN_PULSES;
            TURNS_BREAK = recipe.TURNS_BREAK;
            PLC_PROG_NR = recipe.PLC_PROG_NR;
            LOG_NO = recipe.LOG_NO;
            Tq_UNIT = recipe.Tq_UNIT;
            SelectedThreadType = (int)recipe.SelectedThreadType;
            Thread_step = recipe.Thread_step;
            PIPE_TYPE = recipe.PIPE_TYPE;

            Box_Moni_Time = recipe.Box_Moni_Time;
            Box_Len_Min = recipe.Box_Len_Min;
            Box_Len_Max = recipe.Box_Len_Max;

            Pre_Moni_Time = recipe.Pre_Moni_Time;
            Pre_Len_Max = recipe.Pre_Len_Max;
            Pre_Len_Min = recipe.Pre_Len_Min;

            MU_Moni_Time = recipe.MU_Moni_Time;
            MU_Tq_Ref = recipe.MU_Tq_Ref;
            MU_Tq_Save = recipe.MU_Tq_Save;

            SelectedMode = (int)recipe.JointMode;

            MU_TqSpeedRed_1 = recipe.MU_TqSpeedRed_1;
            MU_TqSpeedRed_2 = recipe.MU_TqSpeedRed_2;
            MU_Tq_Dump = recipe.MU_Tq_Dump;
            MU_Tq_Max = recipe.MU_Tq_Max;
            MU_Tq_Min = recipe.MU_Tq_Min;
            MU_Tq_Opt = recipe.MU_Tq_Opt;

            MU_TqShoulder_Min = recipe.MU_TqShoulder_Min;
            MU_TqShoulder_Max = recipe.MU_TqShoulder_Max;

            MU_Len_Speed_1 = recipe.MU_Len_Speed_1;
            MU_Len_Speed_2 = recipe.MU_Len_Speed_2;
            MU_Len_Dump = recipe.MU_Len_Dump;
            MU_Len_Min = recipe.MU_Len_Min;
            MU_Len_Max = recipe.MU_Len_Max;

            MU_JVal_Speed_1 = recipe.MU_JVal_Speed_1;
            MU_JVal_Speed_2 = recipe.MU_JVal_Speed_2;
            MU_JVal_Dump = recipe.MU_JVal_Dump;
            MU_JVal_Min = recipe.MU_JVal_Min;
            MU_JVal_Max = recipe.MU_JVal_Max;

            TimeStamp = recipe.TimeStamp;

            return this;
            

        }

        public JointRecipe ToJointRecipe()
        {
            return new JointRecipe()
            {
                Id = this.Id,
                Name = this.Name,
                HEAD_OPEN_PULSES = (float)this.HEAD_OPEN_PULSES,
                TURNS_BREAK = (float)this.TURNS_BREAK,
                PLC_PROG_NR = (ushort)this.PLC_PROG_NR,
                LOG_NO = (ushort)this.LOG_NO,
                Tq_UNIT = (ushort)this.Tq_UNIT,
                SelectedThreadType = (ThreadType)this.SelectedThreadType,
                Thread_step = (float)this.Thread_step,
                PIPE_TYPE = this.PIPE_TYPE,

                Box_Moni_Time = (int)this.Box_Moni_Time,
                Box_Len_Min = (float)this.Box_Len_Min,
                Box_Len_Max = (float)this.Box_Len_Max,

                Pre_Moni_Time = (int)this.Pre_Moni_Time,
                Pre_Len_Max = (float)this.Pre_Len_Max,
                Pre_Len_Min = (float)this.Pre_Len_Min,

                MU_Moni_Time = (int)this.MU_Moni_Time,
                MU_Tq_Ref = (float)this.MU_Tq_Ref,
                MU_Tq_Save = (float)this.MU_Tq_Save,

                JointMode = (JointMode)this.SelectedMode,

                MU_TqSpeedRed_1 = (float)this.MU_TqSpeedRed_1,
                MU_TqSpeedRed_2 = (float)this.MU_TqSpeedRed_2,
                MU_Tq_Dump = (float)this.MU_Tq_Dump,
                MU_Tq_Max = (float)this.MU_Tq_Max,
                MU_Tq_Min = (float)this.MU_Tq_Min,
                MU_Tq_Opt = (float)this.MU_Tq_Opt,

                MU_TqShoulder_Min = (float)this.MU_TqShoulder_Min,
                MU_TqShoulder_Max = (float)this.MU_TqShoulder_Max,

                MU_Len_Speed_1 = (float)this.MU_Len_Speed_1,
                MU_Len_Speed_2 = (float)this.MU_Len_Speed_2,
                MU_Len_Dump = (float)this.MU_Len_Dump,
                MU_Len_Min = (float)this.MU_Len_Min,
                MU_Len_Max = (float)this.MU_Len_Max,

                MU_JVal_Speed_1 = (float)this.MU_JVal_Speed_1,
                MU_JVal_Speed_2 = (float)this.MU_JVal_Speed_2,
                MU_JVal_Dump = (float)this.MU_JVal_Dump,
                MU_JVal_Min = (float)this.MU_JVal_Min,
                MU_JVal_Max = (float)this.MU_JVal_Max,

                TimeStamp = this.TimeStamp
            };
        }

        public void CopyProperties(JointRecipeTable recipe)
        {
            if (recipe == null) throw new ArgumentNullException("Recipe is Null");

            Name = recipe.Name;
            HEAD_OPEN_PULSES = recipe.HEAD_OPEN_PULSES;
            TURNS_BREAK = recipe.TURNS_BREAK;
            PLC_PROG_NR = recipe.PLC_PROG_NR;
            LOG_NO = recipe.LOG_NO;
            Tq_UNIT = recipe.Tq_UNIT;
            SelectedThreadType = (int)recipe.SelectedThreadType;
            Thread_step = recipe.Thread_step;
            PIPE_TYPE = recipe.PIPE_TYPE;

            Box_Moni_Time = recipe.Box_Moni_Time;
            Box_Len_Min = recipe.Box_Len_Min;
            Box_Len_Max = recipe.Box_Len_Max;

            Pre_Moni_Time = recipe.Pre_Moni_Time;
            Pre_Len_Max = recipe.Pre_Len_Max;
            Pre_Len_Min = recipe.Pre_Len_Min;

            MU_Moni_Time = recipe.MU_Moni_Time;
            MU_Tq_Ref = recipe.MU_Tq_Ref;
            MU_Tq_Save = recipe.MU_Tq_Save;

            SelectedMode = recipe.SelectedMode;

            MU_TqSpeedRed_1 = recipe.MU_TqSpeedRed_1;
            MU_TqSpeedRed_2 = recipe.MU_TqSpeedRed_2;
            MU_Tq_Dump = recipe.MU_Tq_Dump;
            MU_Tq_Max = recipe.MU_Tq_Max;
            MU_Tq_Min = recipe.MU_Tq_Min;
            MU_Tq_Opt = recipe.MU_Tq_Opt;

            MU_TqShoulder_Min = recipe.MU_TqShoulder_Min;
            MU_TqShoulder_Max = recipe.MU_TqShoulder_Max;

            MU_Len_Speed_1 = recipe.MU_Len_Speed_1;
            MU_Len_Speed_2 = recipe.MU_Len_Speed_2;
            MU_Len_Dump = recipe.MU_Len_Dump;
            MU_Len_Min = recipe.MU_Len_Min;
            MU_Len_Max = recipe.MU_Len_Max;

            MU_JVal_Speed_1 = recipe.MU_JVal_Speed_1;
            MU_JVal_Speed_2 = recipe.MU_JVal_Speed_2;
            MU_JVal_Dump = recipe.MU_JVal_Dump;
            MU_JVal_Min = recipe.MU_JVal_Min;
            MU_JVal_Max = recipe.MU_JVal_Max;

            TimeStamp = recipe.TimeStamp;
            RemovedDate = recipe.RemovedDate;
        }
    }
}
