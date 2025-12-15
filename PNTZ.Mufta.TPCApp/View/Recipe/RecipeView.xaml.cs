using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Linq;

using static PNTZ.Mufta.TPCApp.App;

namespace PNTZ.Mufta.TPCApp.View.Recipe
{
    /// <summary>
    /// Interaction logic for CreateRecipeView.xaml
    /// </summary>
    public partial class RecipeView : UserControl
    {

        public RecipeView()
        {
            InitializeComponent();

            XDocument config = XDocument.Load($"{AppInstance.CurrentDirectory}/View/ViewConfig.xml");
            
            floatRegexAllow = new Regex(config.Root.Element("InputFloatRegex").Value);
            intRegexAllow = new Regex(config.Root.Element("InputIntRegex").Value);
            
            RecipeNameRegexAllow = new Regex(config.Root.Element("InputRecipeNameRegex").Value);

        }

        private void RecipeNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateRecipeNameWatermark();
        }

        private void UpdateRecipeNameWatermark()
        {
            RecipeNameWatermark.Visibility = string.IsNullOrWhiteSpace(RecipeNameTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateRecipeNameWatermark(); // на старте проверить
        }

        private readonly Regex floatRegexAllow;
        private readonly Regex intRegexAllow;
        private readonly Regex RecipeNameRegexAllow;

        private void CheckTextForRegex(TextBox sender, TextCompositionEventArgs e, Regex regex)
        {
            try
            {
                string sourceText = sender.Text;
                int insertionIndex = sender.CaretIndex;
                string newChars = e.Text;

                StringBuilder builder = new StringBuilder(sourceText);
                builder.Insert(insertionIndex, newChars);

                string resultLine = builder.ToString();

                if (regex.Replace(resultLine, "", 1).Trim() != "")
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось проверить ввод: " + ex.Message);
                e.Handled = false;
            }
        }

        private void FloatInputPreview(object sender, TextCompositionEventArgs e)
        {
            CheckTextForRegex(sender as TextBox, e, floatRegexAllow);
        }

        private void IntInputPreview(object sender, TextCompositionEventArgs e)
        {
            CheckTextForRegex(sender as TextBox, e, intRegexAllow);
        }

        private void RecipeNameInputPreview(object sender, TextCompositionEventArgs e)
        {
            CheckTextForRegex(sender as TextBox, e, RecipeNameRegexAllow);
        }                

    }    
}
