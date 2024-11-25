using System;

using static PNTZ.Mufta.TPCApp.App;


namespace PNTZ.Mufta.TPCApp.Domain
{

    public class JointRecipe
    {

        DomainObjectXmlConfigurator<JointRecipe> Configurator;
        public JointRecipe()
        {
            Configurator = new DomainObjectXmlConfigurator<JointRecipe>();

        }
       
        string name;
        public string Name 
        {
            get => name;            
            set
            {
                if (value.Length < 10)
                {
                    name = value;
                }
                else
                    throw new ArgumentException("Значение имени должно быть меньше 10 символов");
            }
        }

        
        // Общие данные
        [ComparableValidationProperty("HEAD_OPEN_PULSES")]
        public float HEAD_OPEN_PULSES { get => Configurator.GetFloatValue<float>(nameof(HEAD_OPEN_PULSES)); set => Configurator.SetFloatValue(nameof(HEAD_OPEN_PULSES), value); }

        [ComparableValidationProperty("TURNS_BREAK")]
        public float TURNS_BREAK { get => Configurator.GetFloatValue<float>(nameof(TURNS_BREAK)); set => Configurator.SetFloatValue(nameof(TURNS_BREAK), value); }
        public ushort PLC_PROG_NR { get; set; }
        public ushort LOG_NO { get; set; }
        public ushort Tq_UNIT { get; set; }
        public ushort Thread_type { get; set; }
        [ComparableValidationProperty("Thread_step")]
        public float Thread_step { get => Configurator.GetFloatValue<float>(nameof(Thread_step)); set => Configurator.SetFloatValue(nameof(Thread_step), value); }
        public string PIPE_TYPE { get; set; }


        // Параметры муфты
        [ComparableValidationProperty("Box_Moni_Time")]
        public int Box_Moni_Time { get => Configurator.GetFloatValue<int>(nameof(Box_Moni_Time)); set => Configurator.SetFloatValue(nameof(Box_Moni_Time), value); }
        [ComparableValidationProperty("Box_Len_Min")]
        public float Box_Len_Min { get => Configurator.GetFloatValue<float>(nameof(Box_Len_Min)); set => Configurator.SetFloatValue(nameof(Box_Len_Min), value); }
        [ComparableValidationProperty("Box_Len_Max")]
        public float Box_Len_Max { get => Configurator.GetFloatValue<float>(nameof(Box_Len_Max)); set => Configurator.SetFloatValue(nameof(Box_Len_Max), value); }


        //Параметры преднавёртки
        [ComparableValidationProperty("Pre_Moni_Time")]
        public int Pre_Moni_Time { get => Configurator.GetFloatValue<int>(nameof(Pre_Moni_Time)); set => Configurator.SetFloatValue(nameof(Pre_Moni_Time), value); }
        [ComparableValidationProperty("Pre_Len_Max")]
        public float Pre_Len_Max { get => Configurator.GetFloatValue<float>(nameof(Pre_Len_Max)); set => Configurator.SetFloatValue(nameof(Pre_Len_Max), value); }
        [ComparableValidationProperty("Pre_Len_Min")]
        public float Pre_Len_Min { get => Configurator.GetFloatValue<float>(nameof(Pre_Len_Min)); set => Configurator.SetFloatValue(nameof(Pre_Len_Min), value); }
        [ComparableValidationProperty("MU_Moni_Time")]


        //Параметры силового свинчивания общие
        public int MU_Moni_Time { get => Configurator.GetFloatValue<int>(nameof(MU_Moni_Time)); set => Configurator.SetFloatValue(nameof(MU_Moni_Time), value); }
        [ComparableValidationProperty("MU_Tq_Ref")]
        public float MU_Tq_Ref { get => Configurator.GetFloatValue<float>(nameof(MU_Tq_Ref)); set => Configurator.SetFloatValue(nameof(MU_Tq_Ref), value); }
        [ComparableValidationProperty("MU_Tq_Save")]
        public float MU_Tq_Save { get => Configurator.GetFloatValue<float>(nameof(MU_Tq_Save)); set => Configurator.SetFloatValue(nameof(MU_Tq_Save), value); }       
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
        public JointMode JointMode { get; set; }
        [ComparableValidationProperty("MU_TqSpeedRed_1")]
        public float MU_TqSpeedRed_1 { get => Configurator.GetFloatValue<float>(nameof(MU_TqSpeedRed_1)); set => Configurator.SetFloatValue(nameof(MU_TqSpeedRed_1), value); }
        [ComparableValidationProperty("MU_TqSpeedRed_2")]
        public float MU_TqSpeedRed_2 { get => Configurator.GetFloatValue<float>(nameof(MU_TqSpeedRed_2)); set => Configurator.SetFloatValue(nameof(MU_TqSpeedRed_2), value); }
        [ComparableValidationProperty("MU_Tq_Dump")]
        public float MU_Tq_Dump { get => Configurator.GetFloatValue<float>(nameof(MU_Tq_Dump)); set => Configurator.SetFloatValue(nameof(MU_Tq_Dump), value); }
        [ComparableValidationProperty("MU_Tq_Max")]
        public float MU_Tq_Max { get => Configurator.GetFloatValue<float>(nameof(MU_Tq_Max)); set => Configurator.SetFloatValue(nameof(MU_Tq_Max), value); }
        [ComparableValidationProperty("MU_Tq_Min")]
        public float MU_Tq_Min { get => Configurator.GetFloatValue<float>(nameof(MU_Tq_Min)); set => Configurator.SetFloatValue(nameof(MU_Tq_Min), value); }
        [ComparableValidationProperty("MU_Tq_Opt")]
        public float MU_Tq_Opt { get => Configurator.GetFloatValue<float>(nameof(MU_Tq_Opt)); set => Configurator.SetFloatValue(nameof(MU_Tq_Opt), value); }
        [ComparableValidationProperty("MU_TqShoulder_Min")]
        public float MU_TqShoulder_Min { get => Configurator.GetFloatValue<float>(nameof(MU_TqShoulder_Min)); set => Configurator.SetFloatValue(nameof(MU_TqShoulder_Min), value); }
        [ComparableValidationProperty("MU_TqShoulder_Max")]
        public float MU_TqShoulder_Max { get => Configurator.GetFloatValue<float>(nameof(MU_TqShoulder_Max)); set => Configurator.SetFloatValue(nameof(MU_TqShoulder_Max), value); }



        //Параметры силового свинчивания по длине
        [ComparableValidationProperty("MU_Len_Speed_1")]
        public float MU_Len_Speed_1 { get => Configurator.GetFloatValue<float>(nameof(MU_Len_Speed_1)); set => Configurator.SetFloatValue(nameof(MU_Len_Speed_1), value); }
        [ComparableValidationProperty("MU_Len_Speed_2")]
        public float MU_Len_Speed_2 { get => Configurator.GetFloatValue<float>(nameof(MU_Len_Speed_2)); set => Configurator.SetFloatValue(nameof(MU_Len_Speed_2), value); }
        [ComparableValidationProperty("MU_Len_Dump")]
        public float MU_Len_Dump { get => Configurator.GetFloatValue<float>(nameof(MU_Len_Dump)); set => Configurator.SetFloatValue(nameof(MU_Len_Dump), value); }
        [ComparableValidationProperty("MU_Len_Min")]
        public float MU_Len_Min { get => Configurator.GetFloatValue<float>(nameof(MU_Len_Min)); set => Configurator.SetFloatValue(nameof(MU_Len_Min), value); }
        [ComparableValidationProperty("MU_Len_Max")]
        public float MU_Len_Max { get => Configurator.GetFloatValue<float>(nameof(MU_Len_Max)); set => Configurator.SetFloatValue(nameof(MU_Len_Max), value); }



        //Параметры силового свинчивания по J
        [ComparableValidationProperty("MU_Jval_Speed_1")]
        public float MU_Jval_Speed_1 { get => Configurator.GetFloatValue<float>(nameof(MU_Jval_Speed_1)); set => Configurator.SetFloatValue(nameof(MU_Jval_Speed_1), value); }
        [ComparableValidationProperty("MU_Jval_Speed_2")]
        public float MU_Jval_Speed_2 { get => Configurator.GetFloatValue<float>(nameof(MU_Jval_Speed_2)); set => Configurator.SetFloatValue(nameof(MU_Jval_Speed_2), value); }
        [ComparableValidationProperty("MU_Jval_Dump")]
        public float MU_Jval_Dump { get => Configurator.GetFloatValue<float>(nameof(MU_Jval_Dump)); set => Configurator.SetFloatValue(nameof(MU_Jval_Dump), value); }
        [ComparableValidationProperty("MU_Jval_Min")]
        public float MU_Jval_Min { get => Configurator.GetFloatValue<float>(nameof(MU_Jval_Min)); set => Configurator.SetFloatValue(nameof(MU_Jval_Min), value); }
        [ComparableValidationProperty("MU_Jval_Max")]
        public float MU_Jval_Max { get => Configurator.GetFloatValue<float>(nameof(MU_Jval_Max)); set => Configurator.SetFloatValue(nameof(MU_Jval_Max), value); }
        
    }    
}
