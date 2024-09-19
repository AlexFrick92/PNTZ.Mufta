
using Desktop.MVVM;

using PNTZ.Mufta.App.Domain.Joint;

using static PNTZ.Mufta.App.Global.Vars;
using static PNTZ.Mufta.App.Global.Methods;

namespace PNTZ.Mufta.App.ViewModel
{
    public class JointViewModel : BaseViewModel
    {
        public JointViewModel() 
        {
            if(LoadedRecipe != null)
                JointRecipe = LoadedRecipe;
            else
                JointRecipe = new JointRecipe();    

            LoadedRecipeUpdated += (s, rec) =>
            {
                JointRecipe = rec;
                OnPropertyChanged(nameof(JointRecipe));
            };
        }

        public JointRecipe JointRecipe { get; set; }
    }
}
