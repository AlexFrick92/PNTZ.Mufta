using System;
using System.IO;
using System.Text.Json;

using PNTZ.Mufta.App.Domain.Joint;

using static PNTZ.Mufta.App.Global.Vars;

namespace PNTZ.Mufta.App.Global
{
    static public class Methods
    {
        static public void SaveJointRecipe(JointRecipe joint)
        {
            if (joint == null)
                throw new ArgumentNullException();

            if (joint.Name.Trim() == "")                
                throw new ArgumentException("Не задано имя рецепта");            

            
            string recipeDirectory = $"{CurrentDirectory}/{RecipeFolder}";

            if (!Directory.Exists(recipeDirectory))
            {
                Directory.CreateDirectory(recipeDirectory);
            }

            string path = $"{recipeDirectory}/{joint.Name}.json";

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                JsonSerializer.Serialize<JointRecipe>(fs, joint);
                Console.WriteLine($"Рецепт: {joint.Name} сохранен в {path}");
            }
        }    
        

        static public ushort JointModeToMakeUpMode(JointMode jointMode)
        {
            switch(jointMode)
            {
                case JointMode.Torque: return 0;

                case JointMode.TorqueShoulder: return 0;

                case JointMode.Length : return 1;

                case JointMode.TorqueLength : return 1;

                case JointMode.Jval: return 2;

                case JointMode.TorqueJVal: return 2;                    
            }

            return 0;
        }
    }
}
