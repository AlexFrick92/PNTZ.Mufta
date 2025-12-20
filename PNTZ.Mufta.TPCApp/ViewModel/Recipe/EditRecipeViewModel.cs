using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Domain.Helpers;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.View.Recipe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PNTZ.Mufta.TPCApp.ViewModel.Recipe
{
    public class EditRecipeViewModel : BaseViewModel
    {
        private JointRecipeTable _editingRecipe;
        private JointRecipeTable _originalRecipe;
        private JointRecipeTable _loadedRecipe;
        private bool _hasChanges;
        private IRecipeTableLoader _loader;

        public EditRecipeViewModel(IRecipeTableLoader loader)
        {
            _loader = loader;

            SetModeCommand = new RelayCommand(SetMode);
            SaveRecipeCommand = new RelayCommand(SaveRecipe, CanSaveRecipe);
            CancelCommand = new RelayCommand(CancelChanges, CanCancelChanges);
            DeleteRecipeCommand = new RelayCommand(DeleteRecipe, CanDeleteRecipe);
        }

        /// <summary>
        /// Событие сохранения рецепта
        /// </summary>
        public event EventHandler<JointRecipeTable> RecipeSaved;

        /// <summary>
        /// Событие отмены изменений
        /// </summary>
        public event EventHandler RecipeCancelled;

        /// <summary>
        /// Событие удаления рецепта
        /// </summary>
        public event EventHandler<JointRecipeTable> RecipeDeleted;

        /// <summary>
        /// Флаг наличия несохранённых изменений
        /// </summary>
        public bool HasChanges
        {
            get => _hasChanges;
            private set
            {
                _hasChanges = value;
                OnPropertyChanged(nameof(HasChanges));
                OnPropertyChanged(nameof(IsRecipeReadyForOperations));
                // Обновляем состояние команд
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Рецепт, который редактируется (копия оригинала)
        /// </summary>
        public JointRecipeTable EditingRecipe
        {
            get => _editingRecipe;
            private set
            {
                _editingRecipe = value;
                OnPropertyChanged(nameof(EditingRecipe));
            }
        }

        /// <summary>
        /// Устанавливает рецепт для редактирования
        /// </summary>
        /// <param name="recipe">Оригинальный рецепт</param>
        public void SetEditingRecipe(JointRecipeTable recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe));

            // Отписываемся от старого рецепта
            if (EditingRecipe != null)
            {
                EditingRecipe.PropertyChanged -= OnEditingRecipePropertyChanged;
            }

            // Сохраняем ссылку на оригинал
            _originalRecipe = recipe;

            // Создаём копию для редактирования
            EditingRecipe = JointRecipeHelper.Clone(recipe);

            // Подписываемся на изменения копии
            if (EditingRecipe != null)
            {
                EditingRecipe.PropertyChanged += OnEditingRecipePropertyChanged;
            }

            // Сбрасываем флаг изменений
            HasChanges = false;

            // Проверяем, является ли этот рецепт загруженным
            OnPropertyChanged(nameof(IsLoadedRecipeCurrent));
            OnPropertyChanged(nameof(IsRecipeReadyForOperations));
        }

        /// <summary>
        /// Устанавливает рецепт, который был загружен в PLC
        /// </summary>
        /// <param name="recipe">Загруженный рецепт</param>
        public void SetLoadedRecipe(JointRecipeTable recipe)
        {
            _loadedRecipe = recipe;
            OnPropertyChanged(nameof(IsLoadedRecipeCurrent));
        }

        /// <summary>
        /// Проверяет, является ли текущий редактируемый рецепт загруженным в PLC
        /// </summary>
        public bool IsLoadedRecipeCurrent
        {
            get
            {
                if (_loadedRecipe == null || _originalRecipe == null)
                    return false;

                // Сравниваем оригинальный рецепт с загруженным
                return JointRecipeHelper.AreEqual(_loadedRecipe, _originalRecipe);
            }
        }

        /// <summary>
        /// Обработчик изменений свойств редактируемого рецепта
        /// </summary>
        private void OnEditingRecipePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Проверяем, действительно ли есть изменения
            HasChanges = !JointRecipeHelper.AreEqual(_originalRecipe, EditingRecipe);
        }

        public ICommand SetModeCommand { get; }

        private void SetMode(object parameter)
        {
            if (parameter is JointMode mode && EditingRecipe != null)
            {
                EditingRecipe.JointMode = mode;
                OnPropertyChanged(nameof(EditingRecipe));
            }
        }

        public ICommand SaveRecipeCommand { get; }

        private bool CanSaveRecipe(object parameter)
        {
            return !HasValidationErrors && EditingRecipe != null && _originalRecipe != null && HasChanges;
        }

        private void SaveRecipe(object parameter)
        {
            if (_originalRecipe == null)
                return;

            // Копируем данные из редактируемой копии обратно в оригинал
            EditingRecipe.CopyRecipeDataTo(_originalRecipe);

            // Сбрасываем флаг изменений
            HasChanges = false;

            // Передаём обновлённый оригинал в событие
            RecipeSaved?.Invoke(this, _originalRecipe);
        }



        public ICommand CancelCommand { get; }

        private bool CanCancelChanges(object parameter)
        {
            return _originalRecipe != null && EditingRecipe != null && HasChanges;
        }

        private void CancelChanges(object parameter)
        {
            if (_originalRecipe == null)
                return;

            // Отписываемся от старой копии
            if (EditingRecipe != null)
            {
                EditingRecipe.PropertyChanged -= OnEditingRecipePropertyChanged;
            }

            // Создаём новую копию из оригинала, отбрасывая все изменения
            EditingRecipe = JointRecipeHelper.Clone(_originalRecipe);

            // Подписываемся на новую копию
            if (EditingRecipe != null)
            {
                EditingRecipe.PropertyChanged += OnEditingRecipePropertyChanged;
            }

            // Сбрасываем флаг изменений
            HasChanges = false;

            // Уведомляем об отмене
            RecipeCancelled?.Invoke(this, EventArgs.Empty);
        }

        public ICommand DeleteRecipeCommand { get; }

        private bool CanDeleteRecipe(object parameter)
        {
            // Можно удалить, если рецепт существует
            return _originalRecipe != null;
        }

        private void DeleteRecipe(object parameter)
        {
            if (_originalRecipe == null)
                return;

            // Генерируем событие удаления с оригинальным рецептом
            RecipeDeleted?.Invoke(this, _originalRecipe);

            // Очищаем редактируемый рецепт
            if (EditingRecipe != null)
            {
                EditingRecipe.PropertyChanged -= OnEditingRecipePropertyChanged;
            }

            EditingRecipe = null;
            _originalRecipe = null;
            HasChanges = false;
        }
        private bool _hasValidationErrors;
        /// <summary>
        /// Флаг наличия ошибок валидации в форме
        /// </summary>
        public bool HasValidationErrors
        {
            get => _hasValidationErrors;
            set
            {
                _hasValidationErrors = value;
                OnPropertyChanged(nameof(HasValidationErrors));
                OnPropertyChanged(nameof(IsRecipeReadyForOperations));
                // Обновляем состояние команды сохранения
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Проверяет, готов ли рецепт к выполнению операций (загрузка, экспорт и т.д.)
        /// </summary>
        public bool IsRecipeReadyForOperations
        {
            get => EditingRecipe != null && !HasValidationErrors && !HasChanges;
        }
    }
}
