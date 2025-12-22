using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Domain
{
    /// <summary>
    /// Работа с актуальным рецептом
    /// Загрузка или выгрузка рецепта
    /// </summary>
    public class ActualRecipe : IRecipeTableLoader, INotifyPropertyChanged
    {
        private readonly RecipeDpWorker _recipeDpWorker;
        public ActualRecipe(RecipeDpWorker recipeDpWorker)
        {
            _recipeDpWorker = recipeDpWorker;
        }
        

        public event EventHandler<JointRecipeTable> RecipeLoaded;
        public event PropertyChangedEventHandler PropertyChanged;

        public async Task LoadRecipeAsync(JointRecipeTable recipe)
        {            
            await _recipeDpWorker.LoadRecipeAsync(recipe);            
            LoadedRecipe = recipe;
            RecipeLoaded?.Invoke(this, recipe); 
        }
        private JointRecipeTable _loadedRecipe;
        public JointRecipeTable LoadedRecipe { get => _loadedRecipe; private set { _loadedRecipe = value; OnPropertyChanged(nameof(LoadedRecipe)); } }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
