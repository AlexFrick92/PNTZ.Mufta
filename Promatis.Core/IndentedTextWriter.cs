using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Promatis.Core
{
    /// <summary>
    /// Формирует текст с отступами.
    /// </summary>
    public class IndentedTextWriter
    {
        private readonly CultureInfo _currentCultureInfo;
        private bool _isNewline = true; // Признак новой строки
        private readonly StringBuilder _textBuilder; 
        private CompilerErrorCollection _errorsField;
        private readonly string _currentIndentString; // Строка отступа от начала строки
        private int _indentLevel;  // Значение уровня отступа
        private readonly List<string> _cachedIndents = new List<string>(); // Кэш сгенерированных отступов

        /// <summary>
        /// Строка отступа по умолчанию.
        /// <remarks>Значение является константой. Для изменения строки отступа, используйте конструктор.</remarks>
        /// </summary>
        public const string DefaultIndentString = "    ";

        #region .ctors

        /// <summary>
        /// Инициализирует новый эеземпляр <see cref="IndentedTextWriter"/>
        /// </summary>
        protected IndentedTextWriter()
        {}

        /// <summary>
        /// Инициализирует новый эеземпляр <see cref="IndentedTextWriter"/>
        /// </summary>
        /// <param name="textBuilder">Экземпляр <see cref="StringBuilder"/></param>
        /// <param name="cultureInfo">Значение <see cref="CultureInfo"/></param>
        public IndentedTextWriter(StringBuilder textBuilder, CultureInfo cultureInfo)
            : this(textBuilder, DefaultIndentString, cultureInfo)
        {
        }

        /// <summary>
        /// Инициализирует новый эеземпляр <see cref="IndentedTextWriter"/>
        /// </summary>
        /// <param name="textBuilder">Экземпляр <see cref="StringBuilder"/></param>
        /// <param name="indentString">Строка, определяющия формат отступа</param>
        /// <param name="cultureInfo">Значение <see cref="CultureInfo"/></param>
        public IndentedTextWriter(StringBuilder textBuilder, string indentString, CultureInfo cultureInfo)
        {
            _currentCultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
            _textBuilder = textBuilder;
            _currentIndentString = indentString;
        }

        #endregion

        /// <summary>
        /// Текст
        /// </summary>
        public string Text => _textBuilder?.ToString() ?? string.Empty;

        /// <summary>
        /// Ошибки
        /// </summary>
        public CompilerErrorCollection Errors => _errorsField ?? (_errorsField = new CompilerErrorCollection());

        #region Indent

        /// <summary>
        /// Добавляет один уровень отступа
        /// </summary>
        public void IndentLevelUp() => _indentLevel++;

        /// <summary>
        /// Убирает один уровень отступа
        /// </summary>
        public void IndentLevelDown() => _indentLevel = _indentLevel <= 0 ? 0 : _indentLevel - 1;

        /// <summary>
        /// Добавляет текущий отступ.
        /// <remarks>Только для новой строки. В противном случае, не добавляется ничего.</remarks>
        /// </summary>
        public void PushIndent()
        {
            if (!_isNewline || _indentLevel <= 0 || string.IsNullOrEmpty(_currentIndentString))
                return;
            _textBuilder.Append(_indentLevel == 1 ? _currentIndentString : GetIndentFromCache());
            _isNewline = false;
        }

        /// <summary>
        /// Возвращает ранее сгенерированный отступ.
        /// <remarks> Если его нет - создает его и помещает в кэш.</remarks>
        /// </summary>
        /// <returns></returns>
        private string GetIndentFromCache()
        {
            var cacheIndex = _indentLevel - 2;
            var cached = cacheIndex < _cachedIndents.Count ? _cachedIndents[cacheIndex] : null;

            if (cached == null)
            {
                cached = BuildIndent(_indentLevel);

                if (cacheIndex == _cachedIndents.Count)
                    _cachedIndents.Add(cached);
                else
                {
                    for (var i = _cachedIndents.Count; i <= cacheIndex; i++)
                    {
                        _cachedIndents.Add(null);
                    }
                    _cachedIndents[cacheIndex] = cached;
                }
            }
            return cached;
        }

        /// <summary>
        /// Строит строку отступа с учетом уровня
        /// </summary>
        /// <param name="level">Уровень</param>
        /// <returns>Строка отступа</returns>
        private string BuildIndent(int level)
        {
            var sb = new StringBuilder(level*_currentIndentString.Length);

            for (var index = 0; index < level; ++index)
            {
                sb.Append(_currentIndentString);
            }

            return sb.ToString();
        }

        #endregion

        #region Write Methods

        /// <summary>
        /// Записывает строку текста.
        /// <remarks>Если <paramref name="textToWrite"/> содержит символы новой строки, то они ингнорируются.</remarks>
        /// </summary>
        /// <param name="textToWrite">Строка для записи.</param>
        public void Write(string textToWrite)
        {
            if (string.IsNullOrEmpty(textToWrite))
                return;
            
            PushIndent();
            
            if (textToWrite.EndsWith(Environment.NewLine, true, _currentCultureInfo))
                textToWrite = textToWrite.Replace(Environment.NewLine, string.Empty);

            _textBuilder.Append(textToWrite);
            _isNewline = false;
        }

        /// <summary>
        /// Записывает строку текста без завершения строки.
        /// </summary>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Аргументы</param>
        public void Write(string format, params object[] args) => Write(string.Format(_currentCultureInfo, format, args));

        /// <summary>
        /// Записывает строку текста с новой строки, добавляя текущий отступ перед текстом.
        /// </summary>
        /// <param name="textToWrite">Строка для записи.</param>
        public void WriteLine(string textToWrite)
        {
            StartNewLine();
            Write(textToWrite);
        }

        /// <summary>
        /// Записывает строку текста с новой строки, добавляя текущий отступ перед текстом.
        /// </summary>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Аргументы</param>
        public void WriteLine(string format, params object[] args) => WriteLine(string.Format(_currentCultureInfo, format, args));

        /// <summary>
        /// Записывает строку текста, заключая ее в условный оператор IF.
        /// <code>If("<paramref name="textToWrite"/>.Exists())</code>
        /// </summary>
        /// <param name="textToWrite">Строка условия</param>
        public void WriteExistingLine(string textToWrite)
        {
            StartNewLine();
            Write("if({0}.Exists())", textToWrite);
        }

        /// <summary>
        /// Записывает строку текста, заключая ее в условный оператор IF.
        /// <code>If("<paramref name="format"/>.Exists())</code>
        /// </summary>
        /// <param name="format">Формат строки условия</param>
        /// <param name="args">Аргументы</param>
        public void WriteExistingLine(string format, params object[] args) => WriteExistingLine(string.Format(_currentCultureInfo, format, args));

        /// <summary>
        /// Записывает символ ;
        /// </summary>
        public void WriteSemicolon() => _textBuilder.Append(";");

        /// <summary>
        /// Записывает символ переноса строки.
        /// </summary>
        public void StartNewLine()
        {
            if (_isNewline)
                return;
            _textBuilder.AppendLine();
            _isNewline = true;
        }

        #endregion

        /// <summary>
        /// Добавляет ошибку
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            Errors.Add(new CompilerError
            {
                ErrorText = message
            });
        }

        /// <summary>
        /// Добавляет предупреждение
        /// </summary>
        /// <param name="message"></param>
        public void Warning(string message)
        {
            Errors.Add(new CompilerError
            {
                ErrorText = message,
                IsWarning = true
            });
        }
    }
}
