using DevExpress.Charts.Designer.Native;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class JointRecipeViewModel : ViewModelBase
    {
        JointRecipe recipe;
        public JointRecipe Recipe { get => recipe; }
        public JointRecipeViewModel(JointRecipe recipe)
        {
            this.recipe = recipe;
        }
        

        public string Name { get => recipe.Name; set => recipe.Name = value; }
        public float HEAD_OPEN_PULSES { get => recipe.HEAD_OPEN_PULSES; set => recipe.HEAD_OPEN_PULSES = value; }
        public float TURNS_BREAK { get => recipe.TURNS_BREAK; set => recipe.TURNS_BREAK = value; }
        public ThreadType SelectedThreadType { get => recipe.SelectedThreadType; set => recipe.SelectedThreadType = value; }
        public float Thread_step { get => recipe.Thread_step; set => recipe.Thread_step = value; }      
        
        public ushort PLC_PROGR_NUM { get => recipe.PLC_PROG_NR; set => recipe.PLC_PROG_NR = value; }



        // Параметры муфты        
        public int Box_Moni_Time { get => recipe.Box_Moni_Time; set => recipe.Box_Moni_Time = value; }        
        public float Box_Len_Min { get => recipe.Box_Len_Min; set => recipe.Box_Len_Min = value; }        
        public float Box_Len_Max { get => recipe.Box_Len_Max; set => recipe.Box_Len_Max = value; }



        //Параметры преднавёртки        
        public int Pre_Moni_Time { get => recipe.Pre_Moni_Time; set => recipe.Pre_Moni_Time = value; }        
        public float Pre_Len_Max { get => recipe.Pre_Len_Max; set => recipe.Pre_Len_Max = value; }        
        public float Pre_Len_Min { get => recipe.Pre_Len_Min; set => recipe.Pre_Len_Min = value; }        



        //Параметры силового свинчивания общие
        public int MU_Moni_Time { get => recipe.MU_Moni_Time; set => recipe.MU_Moni_Time = value; }        
        public float MU_Tq_Ref { get => recipe.MU_Tq_Ref; set => recipe.MU_Tq_Ref = value; }        
        public float MU_Tq_Save { get => recipe.MU_Tq_Save; set => recipe.MU_Tq_Save = value; }
        public JointMode JointMode { get => recipe.JointMode; set => recipe.JointMode = value; }
        public float MU_TqSpeedRed_1 { get => recipe.MU_TqSpeedRed_1; set => recipe.MU_TqSpeedRed_1 = value; }
        public float MU_TqSpeedRed_2 { get => recipe.MU_TqSpeedRed_2; set => recipe.MU_TqSpeedRed_2 = value; }        
        public float MU_Tq_Dump { get => recipe.MU_Tq_Dump; set => recipe.MU_Tq_Dump = value; }        
        public float MU_Tq_Max { get => recipe.MU_Tq_Max; set => recipe.MU_Tq_Max = value; }        
        public float MU_Tq_Min { get => recipe.MU_Tq_Min; set => recipe.MU_Tq_Min = value; }        
        public float MU_Tq_Opt { get => recipe.MU_Tq_Opt; set => recipe.MU_Tq_Opt = value; }            
        public float MU_TqShoulder_Min { get => recipe.MU_TqShoulder_Min; set => recipe.MU_TqShoulder_Min = value; }            
        public float MU_TqShoulder_Max { get => recipe.MU_TqShoulder_Max; set => recipe.MU_TqShoulder_Max = value; }



        //Параметры силового свинчивания по длине        
        public float MU_Len_Speed_1 { get => recipe.MU_Len_Speed_1; set => recipe.MU_Len_Speed_1 = value; }        
        public float MU_Len_Speed_2 { get => recipe.MU_Len_Speed_2; set => recipe.MU_Len_Speed_2 = value; }
        
        public float MU_Len_Dump { get => recipe.MU_Len_Dump; set => recipe.MU_Len_Dump = value; }
        
        public float MU_Len_Min { get => recipe.MU_Len_Min; set => recipe.MU_Len_Min = value; }
        
        public float MU_Len_Max { get => recipe.MU_Len_Max; set => recipe.MU_Len_Max = value; }



        //Параметры силового свинчивания по J        
        public float MU_Jval_Speed_1 { get => recipe.MU_JVal_Speed_1; set => recipe.MU_JVal_Speed_1 = value; }        
        public float MU_Jval_Speed_2 { get => recipe.MU_JVal_Speed_2; set => recipe.MU_JVal_Speed_2 = value; }        
        public float MU_Jval_Dump { get => recipe.MU_JVal_Dump; set => recipe.MU_JVal_Dump = value; }        
        public float MU_Jval_Min { get => recipe.MU_JVal_Min; set => recipe.MU_JVal_Min = value; }        
        public float MU_Jval_Max { get => recipe.MU_JVal_Max; set => recipe.MU_JVal_Max = value; }



        public DateTime TimeStamp
        {
            get => recipe.TimeStamp;
            set
            {
                recipe.TimeStamp = value;
                OnPropertyChanged(nameof(TimeStamp));
            }
        }

    }

}
