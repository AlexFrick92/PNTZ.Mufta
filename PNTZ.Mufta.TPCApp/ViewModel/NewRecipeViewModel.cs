using Desktop.MVVM;
using DevExpress.Xpf.Charts;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class NewRecipeViewModel : BaseViewModel
    {


        public NewRecipeViewModel()
        {
            CreateRecipeCmd = new RelayCommand((arg) =>
            {
                try
                {
                    Recipe.Name = RecipeName;
                    RecipeCreated?.Invoke(this, Recipe); 
                }
                catch(Exception ex)
                {
                    Error = ex.Message;
                    OnPropertyChanged(nameof(Error));
                }

            });

            CancelCmd = new RelayCommand((arg) =>
            {
                Canceled?.Invoke(this, Recipe);
            });
        }

        public event EventHandler<JointRecipe> RecipeCreated;

        public event EventHandler<JointRecipe> Canceled;
        public ICommand CreateRecipeCmd { get; set; }
        public ICommand CancelCmd { get; set; }
        public JointRecipe Recipe { get; set; } = new JointRecipe();


        public string RecipeName { get; set; }

        public string Error { get; set; }

    }
}
