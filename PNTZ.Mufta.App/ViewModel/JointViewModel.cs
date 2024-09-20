
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

            if(AppInstance.LastJointResult != null)
                JointResult = AppInstance.LastJointResult;
            else
                JointResult = new JointResult();

            AppInstance.PropertyChanged += (s, rec) =>
            {
                if(rec.PropertyName == nameof(App.LoadedRecipe))
                {
                    JointRecipe = AppInstance.LoadedRecipe;
                    OnPropertyChanged(nameof(JointRecipe));
                }
                if(rec.PropertyName == nameof(App.LastJointResult))
                {
                    JointResult = AppInstance.LastJointResult;
                    OnPropertyChanged(nameof(JointResult));
                }
            };            
        }

        public JointRecipe JointRecipe { get; set; }

        public JointResult JointResult { get; set; }
    }
}
