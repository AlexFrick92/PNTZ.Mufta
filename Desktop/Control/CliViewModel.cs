using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Desktop.MVVM;
using Toolkit.IO;

namespace Desktop.Control
{
    public class CliViewModel : BaseViewModel
    {

        public CliViewModel(ICliUser cli)
        {
            this.cli = cli;
            cli.NewLineAdded += (s, t) =>
            {

                AddLogLine(t);
            };
            
            foreach(var line in cli.ReadAndCloseBuffer())
            {
                AddLogLine(line);
            }

            EnterInput = new RelayCommand(obj => 
            {
                cli.EnterInput(Input);
                Input = "";     
            });
        }

        private readonly int MaxLines = 500;
        private readonly Queue<string> _logLines = new Queue<string>();

        public string Output => string.Join(Environment.NewLine, _logLines);

        public void AddLogLine(string line)
        {
            _logLines.Enqueue(line);
            while (_logLines.Count > MaxLines)
                _logLines.Dequeue();            
            OnPropertyChanged(nameof(Output));
        }

        ICliUser cli;

        //string output = "";
        //public string Output { get { return output; } set { output = value; OnPropertyChanged(nameof(Output)); } }

        string input = "";
        public string Input { get { return input; } set { input = value; OnPropertyChanged(nameof(Input)); } } 
        
        public ICommand EnterInput { get; private set; } 
    }
}
