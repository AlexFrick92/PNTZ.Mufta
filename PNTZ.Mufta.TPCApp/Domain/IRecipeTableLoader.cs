using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Domain
{
    public interface IRecipeTableLoader
    {
        JointRecipeTable LoadedRecipe { get; }

        event EventHandler<JointRecipeTable> RecipeLoaded;        

        Task LoadRecipeAsync(JointRecipeTable recipe);

    }
}
