using PNTZ.Mufta.TPCApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PNTZ.Mufta.TPCApp.View.Results
{
    /// <summary>
    /// Interaction logic for JointResultsView.xaml
    /// </summary>
    public partial class JointResultsView : UserControl
    {
        public JointResultsView()
        {
            InitializeComponent();
        }
        private void MyComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = GetComboBoxTextBox(RecipeComboBox);
            if (textBox != null)
            {
                textBox.TextChanged += ComboBox_TextChanged;
            }

            UpdateWatermarkVisibility();
        }

        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWatermarkVisibility();
        }

        private void UpdateWatermarkVisibility()
        {
            var text = RecipeComboBox.Text;
            RecipeNameWatermark.Visibility = string.IsNullOrWhiteSpace(text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        private TextBox GetComboBoxTextBox(ComboBox comboBox)
        {
            return FindVisualChild<TextBox>(comboBox);
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
