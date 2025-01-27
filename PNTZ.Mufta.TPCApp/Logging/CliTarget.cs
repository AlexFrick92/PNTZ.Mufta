
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using Toolkit.IO;


namespace PNTZ.Mufta.TPCApp.Logging
{
    [Target("CliTarget")]
    public class CliTarget : TargetWithLayout
    {

        public CliTarget(string name , ICliProgram cliUser)
        {
            this.Name = name;
            cli = cliUser;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            // Форматируем лог
            string logMessage = Layout.Render(logEvent);

            cli.WriteLine(logMessage);
            
        }

        ICliProgram cli;
    }
}
