
using Promatis.Core.Logging;

using PNTZ.Mufta.App.Domain.Joint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.IO;

namespace PNTZ.Mufta.App.Global
{
    static public class Vars
    {
        static public string CurrentDirectory {  get; set; }
        static public string RecipeFolder { get; set; }

        static public RecipeLoader CamRecipeLoader { get; set; }
        
        static public Cli AppCli { get; set; }

        static public ILogger AppLogger {  get; set; }

        static public JointRecipe LoadedRecipe { get; set; }
    }
}
