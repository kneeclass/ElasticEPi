using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Logging;

namespace ElasticEPi.Logging
{
    internal class Logger {

        public static ILogger Log => _logger ?? (_logger = LogManager.Instance.GetLogger("ElasticEPi"));
        private static ILogger _logger;

        public static void WriteToLog(string message, Level level = Level.Debug, Exception exception = null) {
            if(exception != null)
                Log.Log(level, message,exception);
            else 
                Log.Log(level,message);

        }


    }
}
