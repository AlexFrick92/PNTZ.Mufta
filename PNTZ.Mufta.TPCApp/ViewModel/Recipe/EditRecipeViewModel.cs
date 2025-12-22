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
        private RevertableJointRecipe _revertableRecipe;
        private RevertableJointRecipe _loadedRecipe;
        private IRecipeTableLoader _loader;

        public EditRecipeViewModel(IRecipeTableLoader loader)
        {
            _loader = loader;

            SaveRecipeCommand = new RelayCommand(SaveRecipe, CanSaveRecipe);
            CancelCommand = new RelayCommand(CancelChanges, CanCancelChanges);
            DeleteRecipeCommand = new RelayCommand(DeleteRecipe, CanDeleteRecipe);
        }

        /// <summary>
        /// Событие сохранения рецепта
        /// </summary>
        public event EventHandler<RevertableJointRecipe> RecipeSaved;

        /// <summary>
        /// Событие отмены изменений
        /// </summary>
        public event EventHandler RecipeCancelled;

        /// <summary>
        /// Событие удаления рецепта
        /// </summary>
        public event EventHandler<RevertableJointRecipe> RecipeDeleted;

        /// <summary>
        /// Флаг наличия несохранённых изменений
        /// </summary>
        public bool HasChanges => _revertableRecipe?.HasChanges ?? false;

        /// <summary>
        /// Рецепт, который редактируется (копия оригинала)
        /// </summary>
        public JointRecipeTable EditingRecipe => _revertableRecipe?.EditingRecipe;

        /// <summary>
        /// Оригинальный рецепт (для отображения изменений)
        /// </summary>
        public JointRecipeTable OriginalRecipe => _revertableRecipe?.OriginalRecipe;

        /// <summary>
        /// Устанавливает рецепт для редактирования
        /// </summary>
        /// <param name="recipe">RevertableJointRecipe для редактирования</param>
        public void SetEditingRecipe(RevertableJointRecipe recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe));

            // Очищаем старый рецепт если нет изменений (оптимизация памяти)
            if (_revertableRecipe != null)
            {
                _revertableRecipe.ClearEditingIfNoChanges();

                // Отписываемся от старого RevertableJointRecipe
                _revertableRecipe.PropertyChanged -= OnRevertableRecipePropertyChanged;
            }

            // Используем переданный RevertableJointRecipe
            _revertableRecipe = recipe;

            // Подписываемся на изменения HasChanges для обновления команд
            _revertableRecipe.PropertyChanged += OnRevertableRecipePropertyChanged;

            // Уведомляем об изменении свойств
            OnPropertyChanged(nameof(EditingRecipe));
            OnPropertyChanged(nameof(OriginalRecipe));
            OnPropertyChanged(nameof(HasChanges));
            OnPropertyChanged(nameof(IsLoadedRecipeCurrent));
            OnPropertyChanged(nameof(IsRecipeReadyForOperations));

            // Обновляем состояние команд
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Устанавливает рецепт, который был загружен в PLC
        /// </summary>
        /// <param name="recipe">Загруженный RevertableJointRecipe</param>
        public void SetLoadedRecipe(RevertableJointRecipe recipe)
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
                if (_loadedRecipe == null || _revertableRecipe == null)
                    return false;

                // Сравниваем RevertableJointRecipe напрямую
                return _loadedRecipe == _revertableRecipe;
            }
        }

        /// <summary>
        /// Обработчик изменений свойств RevertableJointRecipe
        /// </summary>
        private void OnRevertableRecipePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Пробрасываем изменения HasChanges и IsRecipeReadyForOperations
            if (e.PropertyName == nameof(RevertableJointRecipe.HasChanges))
            {
                OnPropertyChanged(nameof(HasChanges));
                OnPropertyChanged(nameof(IsRecipeReadyForOperations));
                // Обновляем состояние команд
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
            // Обрабатываем изменение IsNew (когда новый рецепт сохранён в БД)
            else if (e.PropertyName == nameof(RevertableJointRecipe.IsNew))
            {
                // HasChanges также может измениться, т.к. его геттер зависит от IsNew
                OnPropertyChanged(nameof(HasChanges));
                OnPropertyChanged(nameof(IsRecipeReadyForOperations));
                // Обновляем состояние команд
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand SaveRecipeCommand { get; }

        private bool CanSaveRecipe(object parameter)
        {
            // Разрешаем сохранение если:
            // 1. Нет ошибок валидации
            // 2. Рецепт существует
            // 3. Есть изменения ИЛИ это новый рецепт
            return !HasValidationErrors && _revertableRecipe != null && (HasChanges || _revertableRecipe.IsNew);
        }

        private void SaveRecipe(object parameter)
        {
            if (_revertableRecipe == null)
                return;

            // Сохраняем изменения в оригинальный рецепт
            _revertableRecipe.Save();

            // Передаём RevertableJointRecipe в событие
            RecipeSaved?.Invoke(this, _revertableRecipe);
        }



        public ICommand CancelCommand { get; }

        private bool CanCancelChanges(object parameter)
        {
            return _revertableRecipe != null && HasChanges;
        }

        private void CancelChanges(object parameter)
        {
            if (_revertableRecipe == null)
                return;

            // Отменяем все изменения
            _revertableRecipe.Revert();

            // Уведомляем об изменении EditingRecipe
            OnPropertyChanged(nameof(EditingRecipe));
            OnPropertyChanged(nameof(OriginalRecipe));

            // Уведомляем об отмене
            RecipeCancelled?.Invoke(this, EventArgs.Empty);
        }

        public ICommand DeleteRecipeCommand { get; }

        private bool CanDeleteRecipe(object parameter)
        {
            // Можно удалить, если рецепт существует
            return _revertableRecipe != null;
        }

        private void DeleteRecipe(object parameter)
        {
            if (_revertableRecipe == null)
                return;

            // Генерируем событие удаления с RevertableJointRecipe
            RecipeDeleted?.Invoke(this, _revertableRecipe);

            // Отписываемся от RevertableJointRecipe
            _revertableRecipe.PropertyChanged -= OnRevertableRecipePropertyChanged;

            // Очищаем
            _revertableRecipe = null;

            // Уведомляем об изменениях
            OnPropertyChanged(nameof(EditingRecipe));
            OnPropertyChanged(nameof(OriginalRecipe));
            OnPropertyChanged(nameof(HasChanges));
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
            get
            {
                // Операции доступны только если:
                // 1. Рецепт существует
                // 2. Нет ошибок валидации
                // 3. Нет несохранённых изменений
                // 4. Рецепт НЕ новый (уже сохранён в БД)
                return EditingRecipe != null
                    && !HasValidationErrors
                    && !HasChanges
                    && _revertableRecipe != null
                    && !_revertableRecipe.IsNew;
            }
        }
    }
}
