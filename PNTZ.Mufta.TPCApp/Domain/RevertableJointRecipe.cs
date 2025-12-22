using PNTZ.Mufta.TPCApp.Domain.Helpers;
using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.ComponentModel;

namespace PNTZ.Mufta.TPCApp.Domain
{
    /// <summary>
    /// Обёртка над JointRecipeTable, которая позволяет отслеживать и отменять изменения
    /// </summary>
    public class RevertableJointRecipe : INotifyPropertyChanged
    {
        private readonly JointRecipeTable _originalRecipe;
        private JointRecipeTable _editingRecipe;
        private bool _hasChanges;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Создаёт новый экземпляр RevertableJointRecipe
        /// </summary>
        /// <param name="originalRecipe">Оригинальный рецепт для редактирования</param>
        /// <exception cref="ArgumentNullException">Если originalRecipe равен null</exception>
        public RevertableJointRecipe(JointRecipeTable originalRecipe)
        {
            if (originalRecipe == null)
                throw new ArgumentNullException(nameof(originalRecipe));

            _originalRecipe = originalRecipe;

            // Копия будет создана при первом обращении к EditingRecipe (lazy loading)
            _editingRecipe = null;

            _hasChanges = false;
        }

        /// <summary>
        /// Оригинальный рецепт (read-only)
        /// </summary>
        public JointRecipeTable OriginalRecipe => _originalRecipe;

        /// <summary>
        /// Редактируемая копия рецепта (создаётся при первом обращении)
        /// </summary>
        public JointRecipeTable EditingRecipe
        {
            get
            {
                // Создаём копию при первом обращении (lazy loading)
                if (_editingRecipe == null)
                {
                    _editingRecipe = JointRecipeHelper.Clone(_originalRecipe);
                    _editingRecipe.PropertyChanged += OnEditingRecipePropertyChanged;
                }
                return _editingRecipe;
            }
        }

        /// <summary>
        /// Флаг наличия несохранённых изменений
        /// Для новых рецептов (IsNew = true) всегда возвращает false,
        /// так как нет базовой версии для сравнения
        /// </summary>
        public bool HasChanges
        {
            get => !IsNew && _hasChanges;
            private set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged(nameof(HasChanges));
                }
            }
        }

        /// <summary>
        /// Проверяет, является ли рецепт новым (ещё не сохранённым в базу данных)
        /// </summary>
        public bool IsNew => _originalRecipe.Id == Guid.Empty;

        /// <summary>
        /// Сохраняет изменения в оригинальный рецепт
        /// </summary>
        public void Save()
        {
            // Копируем данные из редактируемой копии в оригинал
            // (включая Id и TimeStamp, которые могли измениться после сохранения в БД)
            _editingRecipe.CopyRecipeDataTo(_originalRecipe);

            // Сбрасываем флаг изменений
            HasChanges = false;

            // Уведомляем об изменении IsNew (для случая, когда новый рецепт был сохранён в БД)
            // После изменения IsNew нужно также обновить HasChanges, т.к. его геттер зависит от IsNew
            OnPropertyChanged(nameof(IsNew));
            OnPropertyChanged(nameof(HasChanges));
        }

        /// <summary>
        /// Отменяет все изменения и восстанавливает состояние из оригинала
        /// </summary>
        public void Revert()
        {
            // Отписываемся от старой копии
            if (_editingRecipe != null)
            {
                _editingRecipe.PropertyChanged -= OnEditingRecipePropertyChanged;
                _editingRecipe = null;
            }

            // Сбрасываем флаг изменений
            HasChanges = false;

            // Уведомляем об изменении EditingRecipe
            // Новая копия будет создана при следующем обращении к EditingRecipe
            OnPropertyChanged(nameof(EditingRecipe));
        }

        /// <summary>
        /// Очищает редактируемую копию если нет несохранённых изменений
        /// Используется для оптимизации памяти
        /// </summary>
        public void ClearEditingIfNoChanges()
        {
            // Если нет изменений и копия существует - удаляем её
            if (!HasChanges && _editingRecipe != null)
            {
                _editingRecipe.PropertyChanged -= OnEditingRecipePropertyChanged;
                _editingRecipe = null;
            }
        }

        /// <summary>
        /// Уведомляет об изменении Id (вызывается после сохранения нового рецепта в БД)
        /// </summary>
        public void NotifyIdChanged()
        {
            // Уведомляем об изменении IsNew (т.к. оно зависит от Id)
            OnPropertyChanged(nameof(IsNew));
            // Также уведомляем об изменении HasChanges (т.к. его геттер зависит от IsNew)
            OnPropertyChanged(nameof(HasChanges));
        }

        /// <summary>
        /// Обработчик изменений свойств редактируемого рецепта
        /// </summary>
        private void OnEditingRecipePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Проверяем, действительно ли есть изменения
            HasChanges = !JointRecipeHelper.AreEqual(_originalRecipe, _editingRecipe);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
