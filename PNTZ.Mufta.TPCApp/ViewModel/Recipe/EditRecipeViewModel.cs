using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.ViewModel.Recipe
{
    public class EditRecipeViewModel : BaseViewModel
    {
        public EditRecipeViewModel()
        {
            
        }
        /// <summary>
        /// Рецепт, который редактируется
        /// </summary>
        public JointRecipe EditingRecipe { get; private set; }

        public void SetEditingRecipe(JointRecipe recipe)
        {
            EditingRecipe = recipe ?? throw new ArgumentNullException(nameof(recipe));
            OnPropertyChanged(nameof(EditingRecipe));
        }

    }
}
