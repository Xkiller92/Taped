using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Taped.Logging
{
    internal class TroubleLog
    {
        private string _path = $"{Directory.GetCurrentDirectory()}\\Tlog.txt";
        internal Stopwatch timer;
        internal TroubleLog()
        {
            timer = Stopwatch.StartNew();
        }

        public void Register(LogType level, string logMessage)
        {
            using StreamWriter stream = new(_path, append: true);

            string logMsg = "";

            switch (level)
            {
                case LogType.trace:
                    logMsg = "[ERROR::trace::";
                    break;
                case LogType.debug:
                    logMsg = "[ERROR::debug::";
                    break;
                case LogType.warning:
                    logMsg = "[WARNING::";
                    break;
                case LogType.info:
                    logMsg = "[INFO::";
                    break;
            }

            logMsg += $"{timer.Elapsed}]:\n>>>{logMessage}";
            stream.WriteLine(logMsg);
        }
    }

    public enum LogType
    {
        trace,
        debug,
        warning,
        info
    }
}
