using Desktop.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Toolkit.IO;

namespace Desktop.Control
{
    public class BaseMainViewModel : BaseViewModel
    {
        protected readonly Cli Cli;
        public CliViewModel CliViewModel { get; private set; }

        public BaseMainViewModel(Cli cli)
        {
            CliViewModel = new CliViewModel(cli);
            Cli = cli;
        }
    }
}
