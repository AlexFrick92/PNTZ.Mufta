using System;

namespace Promatis.Core.CommandLine
{
    /// <summary>
    /// »сключение, возникающее при работе с командной строкой
    /// </summary>
    public class CommandLineException : Exception
    {
        /// <summary>
        /// »нициализирует новый экземпл€р <see cref="CommandLineException"/>
        /// </summary>
        /// <param name="message">—ообщение</param>
        public CommandLineException(string message) : base(message)
        {
        }

        /// <summary>
        /// »нициализирует новый экземпл€р <see cref="CommandLineException"/>
        /// </summary>
        /// <param name="message">—ообщение</param>
        /// <param name="inner">¬нутреннее исключение</param>
        public CommandLineException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
