using Desktop.MVVM;
using DevExpress.Xpf.Charts;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
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

        public event EventHandler<JointRecipeTable> RecipeCreated;

        public event EventHandler<JointRecipeTable> Canceled;
        public ICommand CreateRecipeCmd { get; set; }
        public ICommand CancelCmd { get; set; }
        public JointRecipeTable Recipe { get; set; } = new JointRecipeTable();


        public string RecipeName { get; set; }

        public string Error { get; set; }

    }
}
