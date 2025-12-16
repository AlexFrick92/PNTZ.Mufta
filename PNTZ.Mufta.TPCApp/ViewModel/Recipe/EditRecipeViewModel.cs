using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PNTZ.Mufta.TPCApp.ViewModel.Recipe
{
    public class EditRecipeViewModel : BaseViewModel
    {
        private JointRecipeViewModel _editingRecipe;

        public EditRecipeViewModel()
        {
            SetModeCommand = new RelayCommand(SetMode);
        }

        /// <summary>
        /// Рецепт, который редактируется
        /// </summary>
        public JointRecipeViewModel EditingRecipe
        {
            get => _editingRecipe;
            private set
            {
                if (_editingRecipe != null)
                {
                    _editingRecipe.PropertyChanged -= OnEditingRecipePropertyChanged;                    
                }

                _editingRecipe = value;

                if (_editingRecipe != null)
                {
                    _editingRecipe.PropertyChanged += OnEditingRecipePropertyChanged;
                    UpdateModeFlags();
                }

                OnPropertyChanged(nameof(EditingRecipe));
            }
        }

        public void SetEditingRecipe(JointRecipe recipe)
        {
            EditingRecipe = new JointRecipeViewModel(recipe) ?? throw new ArgumentNullException(nameof(recipe));
        }

        #region Команды

        public ICommand SetModeCommand { get; }

        private void SetMode(object parameter)
        {
            if (parameter is JointMode mode && EditingRecipe != null)
            {
                EditingRecipe.JointMode = mode;
            }
        }

        #endregion

        #region Флаги режимов

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

        #endregion

        #region Validation

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
            }
        }

        #endregion

        private void OnEditingRecipePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(JointRecipe.JointMode))
            {
                UpdateModeFlags();
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
