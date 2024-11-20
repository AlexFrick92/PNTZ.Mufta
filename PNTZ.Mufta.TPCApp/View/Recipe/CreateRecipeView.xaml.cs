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

namespace PNTZ.Mufta.TPCApp.View.Recipe
{
    /// <summary>
    /// Interaction logic for CreateRecipeView.xaml
    /// </summary>
    public partial class CreateRecipeView : UserControl
    {

        TextBox[,] grid;
        int gridLen0;
        int gridLen1;

        public CreateRecipeView()
        {
            InitializeComponent();
            
            grid = MakeNavigationGraph();
            gridLen0 = grid.GetLength(0);
            gridLen1 = grid.GetLength(1);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return _regex.IsMatch(text);
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }
        private void InputField_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.Down: 
                    case Key.Up: 
                    case Key.Left: 
                    case Key.Right:
                        ChangeFocus(sender as TextBox, e.Key);
                        break;

                }    
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось переместить фокус: " + ex.Message);
            }
                
        }

        TextBox[,] MakeNavigationGraph()
        {
            TextBox[,] graph = new TextBox[,]
            {
                { Input_RecipeName,     null },                
                { Input_HeadOpen,       Input_BoxMax, },
                { Input_TurnsBreak,     Input_BoxMin },
                { Input_ThreadType,     Input_BoxTime, },
                { Input_Thread_step,    null, },
            };


            foreach (TextBox tb in graph)
            {
                if (tb != null)
                    tb.GotFocus += (s, e) =>
                    {
                        (s as TextBox).SelectAll();
                        e.Handled = true;
                    };
            }

            return graph;                 
        }

        void ChangeFocus(TextBox source, Key dir)
        {

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++) 
                {                    
                    if (grid[i, j] == source)
                    {
                        ChangeFocusFrom(i, j, dir);
                        break;
                    }
                }
            }

        }

        void ChangeFocusFrom(int i, int j, Key dir)
        {
            switch (dir)
            {
                case Key.Down:
                    if (i < gridLen0 - 1)
                    {
                        if (grid[i + 1, j] != null)
                            grid[i + 1, j].Focus();                        
                        else
                            ChangeFocusFrom(i + 1, j, dir);
                    }
                    else
                        if(grid[0, j] != null)
                            grid[0, j].Focus();
                        else
                            ChangeFocusFrom(0, j, dir);
                    break;

                case Key.Up:
                    if (i >= 1)
                    {
                        if (grid[i - 1, j] != null)
                            grid[i - 1, j].Focus();
                        else
                            ChangeFocusFrom(i - 1, j, dir);
                    }
                    else
                        if(grid[gridLen0 - 1, j] != null)
                            grid[gridLen0 - 1, j].Focus();
                        else
                            ChangeFocusFrom(gridLen0 - 1, j, dir);
                    break;

                case Key.Left:
                    if (j >= 1)
                    {
                        if (grid[i, j - 1] != null)
                        {
                            grid[i, j - 1].Focus();                            
                        }
                        else
                            ChangeFocusFrom(i, j - 1, dir);
                    }
                    else
                        if (grid[i, gridLen1 - 1] != null)
                            grid[i, gridLen1 - 1].Focus();
                        else
                            ChangeFocusFrom(i, gridLen1 - 1, dir);
                    break;

                case Key.Right:
                    if (j < gridLen1 - 1)
                        if (grid[i, j + 1] != null)
                            grid[i, j + 1].Focus();
                        else
                            ChangeFocusFrom(i, j + 1, dir);
                    else
                        if (grid[i, 0] != null)
                            grid[i, 0].Focus(); 
                        else
                            ChangeFocusFrom(i, 0, dir);
                    break;
            }            
        }
    }
}
