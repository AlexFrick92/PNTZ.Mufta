using PNTZ.Mufta.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PNTZ.Mufta.Domain.RecipeHandling
{
    internal class RecipeCreator
    {
        public  JointRecipe Recipe { get; private set; }
        public void FromFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                JointRecipe jointRecipe = JsonSerializer.Deserialize<JointRecipe>(fs);
            }
        }
    }
}
