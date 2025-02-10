using System;

namespace PNTZ.Mufta.TPCApp.Domain
{
    public interface IRecipeLoader
    {
        JointRecipe LoadedRecipe { get; }

        event EventHandler<JointRecipe> RecipeLoaded;
    }
}
