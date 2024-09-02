


using System.IO;
using System.Text.Json;

namespace PNTZ.Mufta.App.Domain.Joint
{
    public class RecipeCreator
    {
        public  JointRecipe Recipe { get; private set; }

        public RecipeCreator(string path)
        {
            Recipe = FromFile(path);
        }

        JointRecipe FromFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                JointRecipe jointRecipe = JsonSerializer.Deserialize<JointRecipe>(fs);
                return jointRecipe;
            }
        }
    }
}
