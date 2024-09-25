
using Desktop.MVVM;

using PNTZ.Mufta.App.Domain.Joint;
using System;
using System.Threading;
using System.Threading.Tasks;
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

            Task refreshOperationValues = RefreshOperationValues();
        }        

        async Task RefreshOperationValues()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    ActualTqTnLen = AppInstance.ActualTqTnLen;                    
                    OnPropertyChanged(nameof(ActualTqTnLen));                    
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
            });
        }

        public JointRecipe JointRecipe { get; set; }

        public JointResult JointResult { get; set; }

        public TqTnLen ActualTqTnLen { get; set; }
    }
}
