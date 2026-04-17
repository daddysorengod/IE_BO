using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogger
{
    public class AppLogger : IAppLogger
    {
        private readonly Logger _log;
        public AppLogger()
        {
            _log = LogManager.GetCurrentClassLogger();
        }
        public void Error(string message)
        {
            _log.Error(message);
        }

        public void Warning(string message)
        {
            _log.Warn(message);
        }

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void Warn(string message)
        {
            _log.Warn(message);
        }

        public void Debug(string message)
        {
            _log.Debug(message);
        }

        public void DebugSql(string message)
        {
            //_log.Debug($"[DEBUG SQL] {message}");
            Console.WriteLine($"[DEBUG SQL] {message}");
        }
    }
}
