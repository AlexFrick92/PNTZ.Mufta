
using Desktop.MVVM;

using PNTZ.Mufta.App.Domain.Joint;

using PNTZ.Mufta.App.Global;

namespace PNTZ.Mufta.App.ViewModel
{
    public class JointViewModel : BaseViewModel
    {
        public JointViewModel() 
        {
            if(AppVars.LoadedRecipe != null)
                JointRecipe = AppVars.LoadedRecipe;
            else
                JointRecipe = new JointRecipe();    

            AppMethods.LoadedRecipeUpdated += (s, rec) =>
            {
                JointRecipe = rec;
                OnPropertyChanged(nameof(JointRecipe));
            };
        }

        public JointRecipe JointRecipe { get; set; }
    }
}
