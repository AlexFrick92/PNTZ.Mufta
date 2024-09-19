
using Desktop.MVVM;

using PNTZ.Mufta.App.Domain.Joint;

using PNTZ.Mufta.App;

namespace PNTZ.Mufta.App.ViewModel
{
    public class JointViewModel : BaseViewModel
    {
        public JointViewModel() 
        {
            if(App.LoadedRecipe != null)
                JointRecipe = App.LoadedRecipe;
            else
                JointRecipe = new JointRecipe();    

            App.LoadedRecipeUpdated += (s, rec) =>
            {
                JointRecipe = rec;
                OnPropertyChanged(nameof(JointRecipe));
            };
        }

        public JointRecipe JointRecipe { get; set; }
    }
}
