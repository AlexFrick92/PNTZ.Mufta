using Desktop.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class RemoveRecipeViewModel : BaseViewModel
    {

        public JointRecipeViewModel Recipe { get; private set; }
        public RemoveRecipeViewModel(JointRecipeViewModel recipe)
        {
            Recipe = recipe ?? throw new ArgumentNullException(nameof(recipe));

            RemoveRecipeCmd = new RelayCommand((arg) =>
            {
                RecipeRemoved?.Invoke(this, EventArgs.Empty);
            });

            CancelCmd = new RelayCommand((arg) =>
            {
                Canceled?.Invoke(this, EventArgs.Empty);
            });
            
        }

        public ICommand RemoveRecipeCmd { get; set; }
        public ICommand CancelCmd { get; set; }

        public event EventHandler<EventArgs> Canceled;

        public event EventHandler<EventArgs> RecipeRemoved;
    }
}
