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
        public bool IsTorqueLengthMode
        {
            get => _isTorqueLengthMode;
            set
            {
                _isTorqueLengthMode = value;
                OnPropertyChanged(nameof(IsTorqueLengthMode));
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

        #region Флаги групп параметров

        /// <summary>
        /// Показывать оптимальный момент (для Torque, TorqueShoulder, Length, TorqueLength)
        /// </summary>
        private bool _showTorqueOptimal;
        public bool ShowTorqueOptimal
        {
            get => _showTorqueOptimal;
            set
            {
                _showTorqueOptimal = value;
                OnPropertyChanged(nameof(ShowTorqueOptimal));
            }
        }

        /// <summary>
        /// Показывать уменьшение скорости по моменту (для Torque, TorqueShoulder)
        /// </summary>
        private bool _showTorqueSpeedReduction;
        public bool ShowTorqueSpeedReduction
        {
            get => _showTorqueSpeedReduction;
            set
            {
                _showTorqueSpeedReduction = value;
                OnPropertyChanged(nameof(ShowTorqueSpeedReduction));
            }
        }

        /// <summary>
        /// Показывать мин/макс момента (для Torque, TorqueShoulder, TorqueLength, TorqueJVal)
        /// </summary>
        private bool _showTorqueMinMax;
        public bool ShowTorqueMinMax
        {
            get => _showTorqueMinMax;
            set
            {
                _showTorqueMinMax = value;
                OnPropertyChanged(nameof(ShowTorqueMinMax));
            }
        }

        /// <summary>
        /// Показывать параметры плеча (только для TorqueShoulder)
        /// </summary>
        private bool _showShoulderParams;
        public bool ShowShoulderParams
        {
            get => _showShoulderParams;
            set
            {
                _showShoulderParams = value;
                OnPropertyChanged(nameof(ShowShoulderParams));
            }
        }

        /// <summary>
        /// Показывать параметры длины (для Length, TorqueLength)
        /// </summary>
        private bool _showLengthParams;
        public bool ShowLengthParams
        {
            get => _showLengthParams;
            set
            {
                _showLengthParams = value;
                OnPropertyChanged(nameof(ShowLengthParams));
            }
        }

        /// <summary>
        /// Показывать параметры J (для Jval, TorqueJVal)
        /// </summary>
        private bool _showJvalParams;
        public bool ShowJvalParams
        {
            get => _showJvalParams;
            set
            {
                _showJvalParams = value;
                OnPropertyChanged(nameof(ShowJvalParams));
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

            IsTorqueMode = EditingRecipe.JointMode == JointMode.Torque;
            IsShoulderMode = EditingRecipe.JointMode == JointMode.TorqueShoulder;
            IsLengthMode = EditingRecipe.JointMode == JointMode.Length;
            IsTorqueLengthMode = EditingRecipe.JointMode == JointMode.TorqueLength;
            IsJvalMode = EditingRecipe.JointMode == JointMode.Jval;
            IsTorqueJvalMode = EditingRecipe.JointMode == JointMode.TorqueJVal;

            // Обновление флагов групп параметров
            ShowTorqueOptimal = IsTorqueMode || IsShoulderMode || IsLengthMode || IsTorqueLengthMode;
            ShowTorqueSpeedReduction = IsTorqueMode || IsShoulderMode;
            ShowTorqueMinMax = IsTorqueMode || IsShoulderMode || IsTorqueLengthMode || IsTorqueJvalMode;
            ShowShoulderParams = IsShoulderMode;
            ShowLengthParams = IsLengthMode || IsTorqueLengthMode;
            ShowJvalParams = IsJvalMode || IsTorqueJvalMode;
        }
    }
}
