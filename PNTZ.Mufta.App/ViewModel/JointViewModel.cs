
using Desktop.MVVM;

using PNTZ.Mufta.App.Domain.Joint;

using static PNTZ.Mufta.App.App;

namespace PNTZ.Mufta.App.ViewModel
{
    public class JointViewModel : BaseViewModel
    {
        public JointViewModel() 
        {
            if(AppInstance.LoadedRecipe != null)
                JointRecipe = AppInstance.LoadedRecipe;
            else
                JointRecipe = new JointRecipe();    

            AppInstance.PropertyChanged += (s, rec) =>
            {
                if(rec.PropertyName == nameof(App.LoadedRecipe))
                {
                    JointRecipe = AppInstance.LoadedRecipe;
                    OnPropertyChanged(nameof(JointRecipe));
                }
            };
        }

        public JointRecipe JointRecipe { get; set; }
    }
}
