using Desktop.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Desktop.Control
{
    /// <summary>
    /// Interaction logic for TopButton.xaml
    /// </summary>
    public partial class TopButton : UserControl
    {
        public TopButton()
        {
            InitializeComponent();
        }
        public string TopLabel
        {
            get { return (string)GetValue(TopLabelProperty); }
            set { SetValue(TopLabelProperty, value); }
        }

        public static DependencyProperty TopLabelProperty =
            DependencyProperty.Register(nameof(TopLabel), typeof(string), typeof(TopButton)
        );

        public string BottomLabel
        {
            get { return (string)GetValue(BottomLabelProperty); }
            set { SetValue(BottomLabelProperty, value); }
        }

        public static DependencyProperty BottomLabelProperty =
            DependencyProperty.Register(nameof(BottomLabel), typeof(string), typeof(TopButton)
        );

        public ImageSource IconImage
        {
            get { return (ImageSource)GetValue(IconImageProperty); }
            set { SetValue(IconImageProperty, value); }
        }

        public static DependencyProperty IconImageProperty =
            DependencyProperty.Register(nameof(IconImage), typeof(ImageSource), typeof(TopButton)
        );


        //КНОПКА
        public static readonly DependencyProperty ButtonCommandProperty =
        DependencyProperty.Register(nameof(ButtonCommand), typeof(ICommand), typeof(TopButton), new PropertyMetadata(null));

        public ICommand ButtonCommand
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }
    }
}
