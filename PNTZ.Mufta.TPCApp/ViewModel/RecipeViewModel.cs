﻿using Desktop.MVVM;
using DevExpress.Charts.Designer.Native;
using DevExpress.Xpo.DB;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.View.Recipe;
using Promatis.Core.Logging;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

using static PNTZ.Mufta.TPCApp.App;
using System.Linq;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    /// <summary>
    /// Выбор, редактирование, сохранение и загрузка рецепта.
    /// 
    /// </summary>
    internal class RecipeViewModel : BaseViewModel, IRecipeLoader
    {
        RecipeDpWorker recipeLoader;
        ILogger logger;
        RepositoryContext repo;

        //IRecipeLoader

        public event EventHandler<JointRecipe> RecipeLoaded;
        public event EventHandler<JointRecipe> RecipeLoadFailed;
       
        public JointRecipe LoadedRecipe { get; private set; }
        
        //---

        public RecipeViewModel(RecipeDpWorker recipeLoader, ILogger logger, RepositoryContext repoContext)
        {
            this.logger = logger;
            this.recipeLoader = recipeLoader;
            repo = repoContext;
            

            SetModeCommand = new RelayCommand((mode) => SetMode((JointMode)mode));

            JointRecipes = new ObservableCollection<JointRecipeViewModel>();
            foreach(JointRecipe jointRecipe in repo.LoadRecipes())
            {
                JointRecipes.Add(new JointRecipeViewModel(jointRecipe));
            }

            LoadRecipeCommand = new RelayCommand((arg) =>
            Task.Run(async () =>
            {
                try
                {
                    //Переделать: 
                    //LoadRecipeAsync должен возвращать загруженный рецепт
                    //Теоретически, так как рецепт загружает recipeLoader, он может его модифицировать?
                    //Поэтму, фактически загруженный рецепт должен быть взят после процедуры загрузки.
                    await recipeLoader.LoadRecipeAsync(EditRecipe.Recipe);

                    //Здесь мы делаем допущение, что загруженный рецепт, это тот, что мы передали в функцию загрузки
                    LoadedRecipe = EditRecipe.Recipe;
                    RecipeLoaded?.Invoke(this, LoadedRecipe);
                }
                catch
                {
                    RecipeLoadFailed?.Invoke(this, EditRecipe.Recipe);
                }

            }));

            NewRecipeCommand = new RelayCommand((arg) =>
            {

                NewRecipeViewModel newRecvm = new NewRecipeViewModel();                
                NewRecipeView newRecipeView = new NewRecipeView(newRecvm);
                newRecvm.RecipeCreated += (o, r) =>
                {
                    EditRecipe = new JointRecipeViewModel(r);                    
                    OnPropertyChanged(nameof(EditRecipe));
                    UpdateEditRecipeField(true);
                    newRecipeView.Close();                    
                };
                newRecvm.Canceled += (o, r) =>
                {
                    newRecipeView.Close();
                };


                newRecipeView.ShowDialog();
            });

            
            SaveRecipe = new RelayCommand((arg) =>
            {
                EditRecipe.TimeStamp = DateTime.UtcNow;
                OnPropertyChanged(nameof(EditRecipe));                

                if(!JointRecipes.Contains(EditRecipe))
                    JointRecipes.Add(EditRecipe);

                var fromIndex = JointRecipes.IndexOf(EditRecipe);
                if(fromIndex > 0)
                    JointRecipes.Move(fromIndex, 0);

                repo.SaveRecipe(EditRecipe.Recipe);
            });

            RemoveRecipe = new RelayCommand((arg) =>
            {
                repo.RemoveRecipe(EditRecipe.Recipe);
                JointRecipes.Remove(EditRecipe);                
            });
        }

        public ObservableCollection<JointRecipeViewModel> JointRecipes { get; set;  }

        JointRecipeViewModel selectedJointRecipe;        

        public JointRecipeViewModel SelectedJointRecipe
        {
            get => selectedJointRecipe;
            set
            {
                selectedJointRecipe = value;
                EditRecipe = value;
                OnPropertyChanged(nameof(EditRecipe));
                UpdateEditRecipeField(value != null);
            }
        }
        void UpdateEditRecipeField(bool value)
        {
            if (value)
                Task.Run(async () =>
                {
                    RecipeEditable = false;
                    OnPropertyChanged(nameof(RecipeEditable));

                    await Task.Delay(100);

                    RecipeEditable = true;
                    OnPropertyChanged(nameof(RecipeEditable));
                });
            else
            {
                RecipeEditable = false;
                OnPropertyChanged(nameof(RecipeEditable));
            }    

        }
        public bool RecipeEditable { get; set; }
        public JointRecipeViewModel EditRecipe { get; set; }

        public ICommand SetModeCommand { get; set; }

        public ICommand LoadRecipeCommand { get; set; }

        public ICommand NewRecipeCommand { get; set; }        

        public ICommand SaveRecipe { get; set; }
        public ICommand RemoveRecipe { get; set; }

        public bool IsVisible { get; set; } = false;        

        void SetMode(JointMode newMode)
        {
            IsVisible = true;
            OnPropertyChanged(nameof(IsVisible));

            EditRecipe.JointMode = newMode;
            OnPropertyChanged(nameof(EditRecipe));            
        }
    }
}
