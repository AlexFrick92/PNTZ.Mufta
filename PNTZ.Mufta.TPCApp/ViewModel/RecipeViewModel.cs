using Desktop.MVVM;

using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.View.Recipe;


namespace PNTZ.Mufta.TPCApp.ViewModel
{
    internal class RecipeViewModel : BaseViewModel
    {
        public JointRecipe EditRecipe { get; set; } = new JointRecipe();
    }
}
