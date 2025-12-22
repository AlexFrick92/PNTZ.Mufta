using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PNTZ.Mufta.TPCApp.ViewModel.Recipe
{
    public class LoadingRecipeViewModel : BaseViewModel
    {
        private readonly IRecipeTableLoader _loader;
        private readonly JointRecipeTable _recipe;

        private string _recipeName;
        private bool _isLoading;
        private bool _isError;
        private bool _isSuccess;
        private string _statusMessage;

        public LoadingRecipeViewModel(IRecipeTableLoader loader, JointRecipeTable recipe)
        {
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _recipe = recipe ?? throw new ArgumentNullException(nameof(recipe));

            _recipeName = recipe.Name ?? "";
            _isLoading = true;
            _statusMessage = "Загружается...";
            CloseCommand = new RelayCommand(Close, CanClose);
        }

        /// <summary>
        /// Название загружаемого рецепта
        /// </summary>
        public string RecipeName
        {
            get => _recipeName;
            set
            {
                _recipeName = value;
                OnPropertyChanged(nameof(RecipeName));
            }
        }

        /// <summary>
        /// Идёт процесс загрузки
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        /// <summary>
        /// Произошла ошибка
        /// </summary>
        public bool IsError
        {
            get => _isError;
            set
            {
                _isError = value;
                OnPropertyChanged(nameof(IsError));
            }
        }

        /// <summary>
        /// Успешная загрузка
        /// </summary>
        public bool IsSuccess
        {
            get => _isSuccess;
            set
            {
                _isSuccess = value;
                OnPropertyChanged(nameof(IsSuccess));
            }
        }

        /// <summary>
        /// Сообщение о статусе загрузки
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        /// <summary>
        /// Команда закрытия окна
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Запрос на закрытие окна
        /// </summary>
        public event EventHandler CloseRequested;

        /// <summary>
        /// Запустить загрузку рецепта
        /// </summary>
        public async Task StartLoadingAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Загружается...";

                // Выполняем загрузку
                await _loader.LoadRecipeAsync(_recipe);

                // Успешная загрузка
                IsLoading = false;
                IsError = false;
                IsSuccess = true;
                StatusMessage = "Загружено успешно!";

                // Автоматически закрываем окно через небольшую задержку
                await Task.Delay(1000);
                CloseRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Ошибка загрузки
                IsLoading = false;
                IsError = true;
                IsSuccess = false;
                StatusMessage = $"Ошибка загрузки: {ex.Message}";
            }
        }

        private bool CanClose(object parameter)
        {
            return !IsLoading;
        }

        private void Close(object parameter)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
