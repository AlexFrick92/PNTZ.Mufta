using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toolkit.IO
{
    public class Cli : ICliUser, ICliProgram
    {
        public Cli()
        {
            this.RegisterCommand("help", (args) =>
            {
                WriteLine("Доступные команды:");
                foreach (var arg in commandDictionary) 
                {
                    WriteLine(arg.Key);
                }
            });
        }
        //Cli User
        public event EventHandler<string> NewLineAdded;



        public void EnterInput(string text)
        {
            CallCommand(text);
        }

        Dictionary<string, CommandExecute> commandDictionary = new Dictionary<string, CommandExecute>();

        //Cli Program
        public void RegisterCommand(string commandName, CommandExecute command = null)
        {
            commandDictionary.Add(commandName, command);
        }    
        public void WriteLine(string line)
        {
            if (LinesBuffer != null)
                LinesBuffer.Add(line);

            if (NewLineAdded != null)
            {
                NewLineAdded?.Invoke(this, line);
            }
            else
                Console.WriteLine(line);
        }

        
        public void CallCommand(string text)
        {
            string cmdTxt = text.Split(' ')[0];

            if (commandDictionary.ContainsKey(cmdTxt))
            {
                NewLineAdded?.Invoke(this, ">" + text);

                Task.Run(() =>
                {
                    try
                    {
                        commandDictionary[cmdTxt]?.Invoke(text.Split().Where((val, i) => i != 0).ToArray());
                    }
                    catch(Exception ex)
                    {
                        NewLineAdded?.Invoke(this, ">" + ex.Message);
                    }
                });
            }
            else
            {
                NewLineAdded?.Invoke(this, $"Команда '{cmdTxt}' не распознана");
            }

            
        }

        IList<string> LinesBuffer = new List<string>();

        public IEnumerable<string> ReadAndCloseBuffer()
        {
            if (LinesBuffer != null)
            {
                var lines = LinesBuffer;

                LinesBuffer = null;

                return lines;
            }
            else
                throw new InvalidOperationException("Буфер закрыт");
        }
    }
}
