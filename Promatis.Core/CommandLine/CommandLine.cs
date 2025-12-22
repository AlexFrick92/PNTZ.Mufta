using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Promatis.Core.Resources;

namespace Promatis.Core.CommandLine
{
    /// <summary>
    /// Вспомогательный класс для работы с аргументами командной строки
    /// </summary>
    public static class CommandLine
    {
        private static CommandLineParameterCollection _parameters;

        /// <summary>
        /// Аргументы командной строки
        /// </summary>
        public static string[] Args
        {
            get
            {
                var commandLineArgs = CommandLineEnvironment.GetCommandLineArgs();
                var args = new string[commandLineArgs.Length - 1];
                for (var i = 0; i < commandLineArgs.Length - 1; i++)
                {
                    args[i] = commandLineArgs[i + 1];
                }
                return args;
            }
        }
        
        /// <summary>
        /// Конфигурирует окружение
        /// </summary>
        /// <param name="config"></param>
        public static void Configure(ICommandLineConfiguration config)
        {
            CommandLineEnvironment.ApplyConfiguration(config);
        }

        /// <summary>
        /// Разбирает аргументы командной строки и преобразует их в экземпляр типа <typeparamref name="TArgs"/>
        /// </summary>
        /// <typeparam name="TArgs">Тип аргументов</typeparam>
        /// <returns></returns>
        public static TArgs Parse<TArgs>() where TArgs : class, new()
        {
            TArgs argument = InitializeNewArgument<TArgs>();

            var args = GetArguments();

            if (CommandLineEnvironment.HelpCodes.Intersect(args.Select(x => x.Key)).Any())
            {
                Console.WriteLine(new CommandLineHelp(typeof(TArgs)).GetHelpText(Console.BufferWidth, 3));
                Pause();
                return null;
            }

            args.ForEach(a => ApplyCommandArgument(a, argument));

            _parameters.VerifyRequiredArguments();

            return argument;
        }

        /// <summary>
        /// Пытается разобрать аргументы переданные в командной строке и сформировать на их основе экземпляр типа <typeparamref name="TArgs"/>
        /// </summary>
        /// <typeparam name="TArgs">Тип аргументов</typeparam>
        /// <returns>Результат разбора</returns>
        public static bool TryParse<TArgs>(out TArgs args) where TArgs : class, new()
        {
            args = null;

            try
            {
                args = Parse<TArgs>();
            }
            catch (CommandLineException ex)
            {
                WriteLineColor(ConsoleColor.Red, ex.Message);
                Console.WriteLine(new CommandLineHelp(typeof(TArgs)).GetHelpText(Console.BufferWidth, 3));
                Pause();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Приостанавливает выполнение до нажатия любой клавиши.
        /// </summary>
        /// <param name="text">Сообщение выводимое в консоль</param>
        /// <param name="color">Цвет сообщения</param>
        public static void Pause(string text = null, ConsoleColor color = ConsoleColor.White)
        {
            text = text ?? Localization.CommandLine_PressAnyKey;
            WriteLineColor(color, text);
            Console.ReadKey(true);
        }

        /// <summary>
        /// Выводит в консоль вопрос и приостанавливает выполнение до нажатия клавиши.
        /// </summary>
        /// <param name="text">Текст вопроса</param>
        /// <param name="yesKey">Сивол, определяющий положительный ответ</param>
        /// <param name="color">Цвет сообщения</param>
        public static bool Question(string text, char yesKey, ConsoleColor color = ConsoleColor.White)
        {
            WriteLineColor(color, text);
            return Console.ReadKey(true).KeyChar.Equals(yesKey);
        }

        /// <summary>
        /// Выводит на консоль форматированную строку заданного цвета
        /// </summary>
        /// <param name="color">Цвет строки</param>
        /// <param name="format">Формат строки</param>
        /// <param name="formatArgs">Аргументы строки</param>
        public static void WriteLineColor(ConsoleColor color, string format, params object[] formatArgs)
        {
            WriteLineColor(Console.WriteLine, Console.WriteLine, color, format, formatArgs);
        }

        /// <summary>
        /// Создает и инициализирует экземпляр типа аргументов командной строки
        /// </summary>
        /// <returns></returns>
        private static TArgs InitializeNewArgument<TArgs>() where TArgs : class, new()
        {
            var argument = new TArgs();
            _parameters = new CommandLineParameterCollection(typeof(TArgs));
            foreach (var parameter in _parameters.Values)
            {
                parameter.SetDefaultValue(argument);
            }
            return argument;
        }

        /// <summary>
        /// Получает коллекцию аргументов из командной строки
        /// </summary>
        /// <returns></returns>
        private static List<CommandLineArgument> GetArguments()
        {
            var arguments = new List<CommandLineArgument>();
            
            var nextPosition = 1;

            arguments.AddRange(
                from arg in Args
                let matches = RegexTokenize.Matches(arg)
                select matches.Count == 1
                    ? new CommandLineArgument(matches[0])
                    : new CommandLineArgument(arg, nextPosition++));

            return arguments;
        }

        /// <summary>
        /// Выражение для разбора строки аргумента
        /// </summary>
        private static Regex RegexTokenize => new Regex(TokenizePattern, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// Шаблон для разбора строки
        /// </summary>
        private static string TokenizePattern => string.Format(
            CommandLineEnvironment.TokenizeExpressionFormat, CommandLineEnvironment.CaseSensitive ? null : "-", string.Join(string.Empty, CommandLineEnvironment.ArgumentSeparators),
            string.Join(string.Empty, CommandLineEnvironment.ValueSeparators));

        /// <summary>
        /// Применяет значения заданые в аргументе командной строки к соответствующему параметру
        /// </summary>
        /// <param name="cmd">Аргумент командной строки</param>
        /// <param name="argument">класс аргументов</param>
        private static void ApplyCommandArgument(CommandLineArgument cmd, object argument)
        {
            var parameter = _parameters.Get(cmd);

            if (parameter == null)
                throw new CommandLineException(string.Format(Localization.CommandLine_ArgumentNotFound, cmd.Key));

            parameter.SetValue(argument, cmd.Value);
        }

        private static void WriteLineColor(Action<string> outputNoParams, Action<string, object[]> outputParams,
            ConsoleColor color, string format, params object[] formatArgs)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            if (formatArgs.Length > 0)
                outputParams(format, formatArgs);
            else
                outputNoParams(format);

            Console.ForegroundColor = prevColor;
        }
    }
}
