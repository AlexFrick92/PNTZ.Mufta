using Promatis.Core.Logging;
using System;
using System.Web;
using Toolkit.IO;

namespace Toolkit.Logging
{
    public class CliLogger : ILogger
    {
        private readonly ICliProgram _cli;
        private readonly ILogger logger;


        public CliLogger(ICliProgram cli)
        {
            _cli = cli;
        }
        public CliLogger(ICliProgram cli, ILogger logger)
        {
            _cli = cli;
            this.logger = logger;            
        }
        public void Debug(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(string format, params object[] args)
        {
            string output = $"{DateTime.Now}   [ERROR]   {format}";
            _cli.WriteLine(output);
            logger?.Error(format, args);
        }

        public void Error(Exception exception, string message = null)
        {
            throw new NotImplementedException();
        }

        public void Error(Exception exception, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Fatal(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Fatal(Exception exception, string message = null)
        {
            throw new NotImplementedException();
        }

        public void Fatal(Exception exception, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public ILogger GetLogger(string loggerName)
        {
            throw new NotImplementedException();
        }

        public void Info(string format, params object[] args)
        {

            string output = $"{DateTime.Now}   [INFO]   {format}";

            _cli.WriteLine(output);
            logger?.Info(format, args);
        }

        public void Trace(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Warn(string format, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
