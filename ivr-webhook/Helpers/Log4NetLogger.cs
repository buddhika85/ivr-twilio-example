using log4net;

namespace ivr_webhook.Helpers
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/use-log4net-in-asp-net-mvc-application/
    /// </summary>
    public class Log4NetLogger
    {
        private static readonly Log4NetLogger _instance = new Log4NetLogger();
        protected ILog monitoringLogger;
        protected static ILog debugLogger;

        private Log4NetLogger()
        {
            monitoringLogger = LogManager.GetLogger("MonitoringLogger");
            debugLogger = LogManager.GetLogger("DebugLogger");
        }



        /// <summary>  
        /// Used to log Debug messages in an explicit Debug Logger  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Debug(string message)
        {
            debugLogger.Debug(message);
        }


        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Debug(string message, System.Exception exception)
        {
            debugLogger.Debug(message, exception);
        }


        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Info(string message)
        {
            _instance.monitoringLogger.Info(message);
        }


        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Info(string message, System.Exception exception)
        {
            _instance.monitoringLogger.Info(message, exception);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Warn(string message)
        {
            _instance.monitoringLogger.Warn(message);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Warn(string message, System.Exception exception)
        {
            _instance.monitoringLogger.Warn(message, exception);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Error(string message)
        {
            _instance.monitoringLogger.Error(message);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Error(string message, System.Exception exception)
        {
            _instance.monitoringLogger.Error(message, exception);
        }


        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Fatal(string message)
        {
            _instance.monitoringLogger.Fatal(message);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Fatal(string message, System.Exception exception)
        {
            _instance.monitoringLogger.Fatal(message, exception);
        }
    }
}