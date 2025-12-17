using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Domain.Helpers;
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
        private JointRecipe _editingRecipe;
        private JointRecipe _originalRecipe;
        private bool _hasChanges;
        private IRecipeLoader _loader;

        public EditRecipeViewModel(IRecipeLoader loader)
        {
            _loader = loader;

            SetModeCommand = new RelayCommand(SetMode);
            SaveRecipeCommand = new RelayCommand(SaveRecipe, CanSaveRecipe);
            CancelCommand = new RelayCommand(CancelChanges, CanCancelChanges);
            LoadRecipeCommand = new RelayCommand(LoadRecipe, CanLoadRecipe);
            DeleteRecipeCommand = new RelayCommand(DeleteRecipe, CanDeleteRecipe);
        }

        /// <summary>
        /// Событие сохранения рецепта
        /// </summary>
        public event EventHandler<JointRecipe> RecipeSaved;

        /// <summary>
        /// Событие отмены изменений
        /// </summary>
        public event EventHandler RecipeCancelled;

        /// <summary>
        /// Событие удаления рецепта
        /// </summary>
        public event EventHandler<JointRecipe> RecipeDeleted;

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
                // Обновляем состояние команд
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Рецепт, который редактируется (копия оригинала)
        /// </summary>
        public JointRecipe EditingRecipe
        {
            get => _editingRecipe;
            private set
            {
                _editingRecipe = value;

                if (_editingRecipe != null)
                {
                    UpdateModeFlags();
                }

                OnPropertyChanged(nameof(EditingRecipe));
            }
        }

        /// <summary>
        /// Устанавливает рецепт для редактирования
        /// </summary>
        /// <param name="recipe">Оригинальный рецепт</param>
        public void SetEditingRecipe(JointRecipe recipe)
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
                UpdateModeFlags();
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

        public ICommand LoadRecipeCommand { get; }

        private bool CanLoadRecipe(object parameter)
        {
            // Можно загрузить, если рецепт существует и нет ошибок валидации
            return EditingRecipe != null && !HasValidationErrors && !HasChanges && _loader != null && !RecipeLoadingInProgress;
        }
        public bool RecipeLoadingInProgress { get; private set; } = false;
        private async void LoadRecipe(object parameter)
        {
            LoadingRecipeView loadingWindow = null;

            try
            {
                RecipeLoadingInProgress = true;
                OnPropertyChanged(nameof(RecipeLoadingInProgress));

                // Создаём и показываем окно загрузки
                var loadingViewModel = new LoadingRecipeViewModel(_originalRecipe?.Name ?? "");
                loadingWindow = new LoadingRecipeView(loadingViewModel);
                loadingWindow.Owner = Application.Current.MainWindow;
                loadingWindow.Show();

                await _loader.LoadRecipeAsync(_originalRecipe);
            }
            finally
            {
                RecipeLoadingInProgress = false;
                OnPropertyChanged(nameof(RecipeLoadingInProgress));

                // Закрываем окно загрузки
                loadingWindow?.Close();
            }
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


        private bool _isTorqueMode;
        public bool IsTorqueMode
        {
            get => _isTorqueMode;
            set
            {
                _isTorqueMode = value;
                OnPropertyChanged(nameof(IsTorqueMode));
            }
        }

        private bool _isShoulderMode;
        public bool IsShoulderMode
        {
            get => _isShoulderMode;
            set
            {
                _isShoulderMode = value;
                OnPropertyChanged(nameof(IsShoulderMode));
            }
        }

        private bool _isLengthMode;
        public bool IsLengthMode
        {
            get => _isLengthMode;
            set
            {
                _isLengthMode = value;
                OnPropertyChanged(nameof(IsLengthMode));
            }
        }

        private bool _isTorqueLengthMode;
        public bool IsTorqueOrTorqueLengthMode
        {
            get => _isTorqueLengthMode;
            set
            {
                _isTorqueLengthMode = value;
                OnPropertyChanged(nameof(IsTorqueOrTorqueLengthMode));
            }
        }

        private bool _isJvalMode;
        public bool IsJvalMode
        {
            get => _isJvalMode;
            set
            {
                _isJvalMode = value;
                OnPropertyChanged(nameof(IsJvalMode));
            }
        }

        private bool _isTorqueJvalMode;
        public bool IsTorqueJvalMode
        {
            get => _isTorqueJvalMode;
            set
            {
                _isTorqueJvalMode = value;
                OnPropertyChanged(nameof(IsTorqueJvalMode));
            }
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
                // Обновляем состояние команды сохранения
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        private void UpdateModeFlags()
        {
            if (EditingRecipe == null) return;

            IsTorqueMode = EditingRecipe.JointMode == JointMode.Torque || EditingRecipe.JointMode == JointMode.TorqueShoulder;
            IsShoulderMode = EditingRecipe.JointMode == JointMode.TorqueShoulder;
            IsLengthMode = EditingRecipe.JointMode == JointMode.Length || EditingRecipe.JointMode == JointMode.TorqueLength;
            IsTorqueOrTorqueLengthMode = IsTorqueMode || IsShoulderMode || EditingRecipe.JointMode == JointMode.TorqueLength;          
        }
    }
}
