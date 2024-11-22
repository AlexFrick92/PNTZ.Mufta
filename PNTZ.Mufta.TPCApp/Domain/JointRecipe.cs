using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static PNTZ.Mufta.TPCApp.App;


namespace PNTZ.Mufta.TPCApp.Domain
{

    

    public class JointRecipe
    {

        XDocument config;

        public JointRecipe()
        {
            config = XDocument.Load($"{AppInstance.CurrentDirectory}/Domain/JointRecipeConfig.xml");


            HEAD_OPEN_PULSES = GetDefaultValueFloat("HEAD_OPEN_PULSES");
            TURNS_BREAK = GetDefaultValueFloat("TURNS_BREAK");
            Thread_step = GetDefaultValueFloat("Thread_step");

            Box_Moni_Time = GetDefaultValueInt("Box_Moni_Time");
            Box_Len_Min = GetDefaultValueInt("Box_Len_Min");
            Box_Len_Max = GetDefaultValueInt("Box_Len_Min");
            
            Pre_Moni_Time = GetDefaultValueInt("Box_Len_Min");
            Pre_Len_Max = GetDefaultValueInt("Box_Len_Min");
            Pre_Len_Min = GetDefaultValueInt("Box_Len_Min");

            MU_Moni_Time = GetDefaultValueInt("Box_Len_Min");

            MU_Tq_Ref = GetDefaultValueInt("Box_Len_Min");
            MU_Tq_Save = GetDefaultValueInt("Box_Len_Min");
            MU_TqSpeedRed_1 = GetDefaultValueInt("Box_Len_Min");
            MU_TqSpeedRed_2 = GetDefaultValueInt("Box_Len_Min");
            MU_Tq_Dump = GetDefaultValueInt("Box_Len_Min");
            MU_Tq_Max = GetDefaultValueInt("Box_Len_Min");
            MU_Tq_Min = GetDefaultValueInt("Box_Len_Min");
            MU_Tq_Opt = GetDefaultValueInt("Box_Len_Min");
            MU_TqShoulder_Min = GetDefaultValueInt("Box_Len_Min");
            MU_TqShoulder_Max = GetDefaultValueInt("Box_Len_Min");

            MU_Len_Speed_1 = GetDefaultValueInt("Box_Len_Min");
            MU_Len_Speed_2 = GetDefaultValueInt("Box_Len_Min");
            MU_Len_Dump = GetDefaultValueInt("Box_Len_Min");
            MU_Len_Min = GetDefaultValueInt("Box_Len_Min");
            MU_Len_Max = GetDefaultValueInt("Box_Len_Min");

            MU_Jval_Speed_1 = GetDefaultValueInt("Box_Len_Min");
            MU_Jval_Speed_2 = GetDefaultValueInt("Box_Len_Min");
            MU_Jval_Dump = GetDefaultValueInt("Box_Len_Min");
            MU_Jval_Min = GetDefaultValueInt("Box_Len_Min");
            MU_Jval_Max = GetDefaultValueInt("Box_Len_Min");
            





        }

        float GetValidatedValue(string configName, float value)
        {
            float min = float.Parse(config.Root.Element(configName).Attribute("Min").Value);
            float max = float.Parse(config.Root.Element(configName).Attribute("Max").Value);

            if (value < min)
                return min;
            else if (value > max)   
                return max;
            else
                return value;
        }
        int GetValidatedValue(string configName, int value)
        {
            int min = int.Parse(config.Root.Element(configName).Attribute("Min").Value);
            int max = int.Parse(config.Root.Element(configName).Attribute("Max").Value);

            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        float GetDefaultValueFloat(string configName)
        {
            return float.Parse(config.Root.Element(configName).Attribute("Default").Value);
        }
        int GetDefaultValueInt(string configName)
        {
            return int.Parse(config.Root.Element(configName).Attribute("Default").Value);
        }

        string name;
        public string Name 
        { 
            get
            {
                return name;
            }
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
        float headOpenPulses;
        public float HEAD_OPEN_PULSES
        {
            get => headOpenPulses;
            set
            {
                headOpenPulses = GetValidatedValue("HEAD_OPEN_PULSES", value);
            }
        }
        float turnsBreak;
        public float TURNS_BREAK
        {
            get => turnsBreak;
            set
            {
                turnsBreak = GetValidatedValue("TURNS_BREAK", value);
            }
        }

        public ushort PLC_PROG_NR { get; set; }
        public ushort LOG_NO { get; set; }
        public ushort Tq_UNIT { get; set; }
        public ushort Thread_type { get; set; }



        float threadStep;
        public float Thread_step
        {
            get => threadStep;
            set
            {
                threadStep = GetValidatedValue("Thread_step", value);
            }
        }
        public string PIPE_TYPE { get; set; }


        int boxMoniTime;
        public int Box_Moni_Time
        {
            get => boxMoniTime;
            set
            {
                boxMoniTime = GetValidatedValue("Box_Moni_Time", value);
            }
        }

        float boxLenMin;
        public float Box_Len_Min
        {
            get => boxLenMin;
            set
            {
                boxLenMin = GetValidatedValue("Box_Len_Min", value);
            }
        }

        float boxLenMax;
        public float Box_Len_Max
        {
            get => boxLenMin;
            set
            {
                boxLenMin = GetValidatedValue("Box_Len_Max", value);
            }
        }

        int preMoniTime;
        public int Pre_Moni_Time
        {
            get => preMoniTime;
            set
            {
                preMoniTime = GetValidatedValue("Pre_Moni_Time", value);
            }
        }

        float preLenMin;
        public float Pre_Len_Min
        {
            get => preLenMin;
            set
            {
                preLenMin = GetValidatedValue("Pre_Len_Min", value);
            }
        }

        float preLenMax;
        public float Pre_Len_Max
        {
            get => preLenMax;
            set
            {
                preLenMax = GetValidatedValue("Pre_Len_Max", value);
            }
        }

        int muMoniTime;
        public int MU_Moni_Time
        {
            get => muMoniTime;
            set
            {
                muMoniTime = GetValidatedValue("MU_Moni_Time", value);
            }
        }

        float muTqRef;
        public float MU_Tq_Ref
        {
            get => muTqRef;
            set
            {
                muTqRef = GetValidatedValue("MU_Tq_Ref", value);
            }
        }

        float muTqSave;
        public float MU_Tq_Save
        {
            get => muTqSave;
            set
            {
                muTqSave = GetValidatedValue("MU_Tq_Save", value);
            }
        }


        public ushort MU_Makeup_Mode { get; set; } // 0 - по моменту, 1 - по длине, 2 - по JVal
        public JointModeEnum JointMode { get; set; }



        float muTqSpeedRed1;
        public float MU_TqSpeedRed_1
        {
            get => muTqSpeedRed1;
            set
            {
                muTqSpeedRed1 = GetValidatedValue("MU_TqSpeedRed_1", value);
            }
        }

        float muTqSpeedRed2;
        public float MU_TqSpeedRed_2
        {
            get => muTqSpeedRed2;
            set
            {
                muTqSpeedRed2 = GetValidatedValue("MU_TqSpeedRed_2", value);
            }
        }

        float muTqDump;
        public float MU_Tq_Dump
        {
            get => muTqDump;
            set
            {
                muTqDump = GetValidatedValue("MU_Tq_Dump", value);
            }
        }

        float muTqMax;
        public float MU_Tq_Max
        {
            get => muTqMax;
            set
            {
                muTqMax = GetValidatedValue("MU_Tq_Max", value);
            }
        }

        float muTqMin;
        public float MU_Tq_Min
        {
            get => muTqMin;
            set
            {
                muTqMin = GetValidatedValue("MU_Tq_Min", value);
            }
        }

        float muLenSpeedRed1;
        public float MU_Len_Speed_1
        {
            get => muLenSpeedRed1;
            set
            {
                muLenSpeedRed1 = GetValidatedValue("MU_Len_Speed_1", value);
            }
        }

        float muLenSpeed2;
        public float MU_Len_Speed_2
        {
            get => muLenSpeed2;
            set
            {
                muLenSpeed2 = GetValidatedValue("MU_Len_Speed_2", value);
            }
        }


        float muLenDump;
        public float MU_Len_Dump
        {
            get => muLenDump;
            set
            {
                muLenDump = GetValidatedValue("MU_Len_Dump", value);
            }
        }

        float muLenMin;
        public float MU_Len_Min
        {
            get => muLenMin;
            set
            {
                muLenMin = GetValidatedValue("MU_Len_Min", value);
            }
        }

        float muLenMax;
        public float MU_Len_Max
        {
            get => muLenMax;
            set
            {
                muLenMax = GetValidatedValue("MU_Len_Max", value);
            }
        }

        float muJvalSpeed1;
        public float MU_Jval_Speed_1
        {
            get => muJvalSpeed1;
            set
            {
                muJvalSpeed1 = GetValidatedValue("MU_Jval_Speed_1", value);
            }
        }

        float muJvalSpeed2;
        public float MU_Jval_Speed_2
        {
            get => muJvalSpeed2;
            set
            {
                muJvalSpeed2 = GetValidatedValue("MU_Jval_Speed_2", value);
            }
        }

        float muJvalDump;
        public float MU_Jval_Dump
        {
            get => muJvalDump;
            set
            {
                muJvalDump = GetValidatedValue("MU_Jval_Speed_2", value);
            }
        }

        float muJvalMin;
        public float MU_Jval_Min
        {
            get => muJvalMin;
            set
            {
                muJvalMin = GetValidatedValue("MU_Jval_Min", value);
            }
        }

        float muJvalMan;
        public float MU_Jval_Max
        {
            get => muJvalMan;
            set
            {
                muJvalMan = GetValidatedValue("MU_Jval_Max", value);
            }
        }

        float muTqOpt;
        public float MU_Tq_Opt
        {
            get => muTqOpt;
            set
            {
                muTqOpt = GetValidatedValue("MU_Tq_Opt", value);
            }
        }

        float muTqShoulderMin;
        public float MU_TqShoulder_Min
        {
            get => muTqShoulderMin;
            set
            {
                muTqShoulderMin = GetValidatedValue("MU_TqShoulder_Min", value);
            }
        }

        float muTqShoulderMax;
        public float MU_TqShoulder_Max
        {
            get => muTqShoulderMax;
            set
            {
                muTqShoulderMax = GetValidatedValue("MU_TqShoulder_Max", value);
            }
        }

        public enum JointModeEnum
        {
            Torque, //Контроль момента
            TorqueShoulder, //Контроль момента и заплечника
            Length, //Контроль длины
            TorqueLength, //Контроль длины и момента
            Jval, //Контроль значения J
            TorqueJVal //Контроль значения J и момента
        }
    }    
}
