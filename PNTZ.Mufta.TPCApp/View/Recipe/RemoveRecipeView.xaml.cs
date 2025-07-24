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
using System.Windows.Shapes;

namespace PNTZ.Mufta.TPCApp.View.Recipe
{
    /// <summary>
    /// Interaction logic for RemoveRecipeView.xaml
    /// </summary>
    public partial class RemoveRecipeView : Window
    {
        public RemoveRecipeView(RemoveRecipeViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
