using System;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Domain
{
    public interface IRecipeLoader
    {
        JointRecipe LoadedRecipe { get; }

        event EventHandler<JointRecipe> RecipeLoaded;
        event EventHandler<JointRecipe> RecipeLoadFailed;

        Task LoadRecipeAsync(JointRecipe recipe);
    }
}
