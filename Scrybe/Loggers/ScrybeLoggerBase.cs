using System.Text.Json.Nodes;

namespace Scrybe.Loggers
{
    internal enum LoggingLevel
    {
        Trace = int.MinValue,
        Debug = 0,
        Verbose = 4,
        Info = 16,
        Warning = 64,
        Error = 256,
        Fatal = 1024,
        None = 4096,
        Force = int.MaxValue
    }

    internal abstract class ScrybeLoggerBase
    {
        protected LoggingLevel LoggingLevel { get; set; }

        protected string? LoggingLevelName { get; private set; }

        protected string LoggerName { get; private set; }


        public ScrybeLoggerBase(LoggingLevel loggingLevel, JsonNode config)
        {
            LoggingLevel = loggingLevel;
            LoggerName = config["Name"]?.ToString() ?? "Logger";
        }


        internal void OnLoggerInitialized()
        {
            LogDebug($"Logger '{LoggerName}' initialized");
        }


        public void LogTrace(object? message)
        {
            LogAtLevel(message, LoggingLevel.Trace);
        }


        public void LogDebug(object? message)
        {
            LogAtLevel(message, LoggingLevel.Debug);
        }


        public void LogVerbose(object? message)
        {
            LogAtLevel(message, LoggingLevel.Verbose);
        }


        public void LogInfo(object? message)
        {
            LogAtLevel(message, LoggingLevel.Info);
        }


        public void LogWarning(object? message)
        {
            LogAtLevel(message, LoggingLevel.Warning);
        }


        public void LogError(object? message)
        {
            LogAtLevel(message, LoggingLevel.Error);
        }


        public void LogError(object? message, Exception exception)
        {
            LogAtLevel(message, LoggingLevel.Error);
            LogException(exception, LoggingLevel.Error);
        }


        public void LogFatal(object? message)
        {
            LogAtLevel(message, LoggingLevel.Fatal);
        }


        public void LogFatal(object? message, Exception exception)
        {
            LogAtLevel(message, LoggingLevel.Fatal);
            LogException(exception, LoggingLevel.Fatal);
        }


        public void ForceLog(object? message)
        {
            LogAtLevel(message, int.MaxValue);
        }


        public void LogCustom(object? message, int loggingLevel)
        {
            LogAtLevel(message, loggingLevel);
        }


        public void LogMethodStart(params object?[] parameters)
        {
            string callingMethod = GetCallingMethod();
            string message = $"Starting method {callingMethod}(";
            if(parameters.Length > 0)
            {
                foreach (var param in parameters)
                {
                    if (param == null)
                    {
                        message += " null,";
                    }
                    else if (param is string paramString)
                    {
                        message += $" \"{paramString}\",";
                    }
                    else
                    {
                        message += $" {param},";
                    }
                }
                message = message[0..^1];
            }
            message += ")";
            LogTrace(message);
        }


        public void LogMethodEnd()
        {
            string callingMethod = GetCallingMethod();
            string message = $"Finishing method {callingMethod}";
            LogTrace(message);
        }


        public void LogMethodEnd(object? returnObject)
        {
            string callingMethod = GetCallingMethod();
            string message = $"Finishing method {callingMethod}, returning ";
            if (returnObject is string returnObjectString)
            {
                message += $"\"{returnObjectString}\"";
            }
            else
            {
                message += $"{returnObject}";
            }
            LogTrace(message);
        }


        public void LogErrorInMethod(Exception exception)
        {
            string callingMethod = GetCallingMethod();
            string message = $"Error thrown in {callingMethod}";
            LogError(message, exception);
        }


        public void LogVariableValue(string variableName, object? variableValue)
        {
            string message = $"{variableName} = {variableValue}";
            LogDebug(message);
        }


        public void LogTimeElapsed(DateTime startTime)
        {
            var timeSpan = DateTime.Now - startTime;
            LogInfo($"Time elapsed: {timeSpan}");
        }


        protected abstract void LogMessage(object? message);


        private void LogAtLevel(object? message, LoggingLevel loggingLevel)
        {
            LogAtLevel(message, (int)loggingLevel);
        }


        private void LogAtLevel(object? message, int loggingLevel)
        {
            if((int)LoggingLevel > loggingLevel)
            {
                return;
            }

            message ??= string.Empty;
            LoggingLevelName = GetLoggingLevelName((int)loggingLevel);
            LogMessage(message);
        }


        private void LogException(Exception exception, LoggingLevel loggingLevel)
        {
            if(exception == null || LoggingLevel > loggingLevel)
            {
                return;
            }

            LogAtLevel(exception.Message, loggingLevel);
            LogAtLevel(exception.StackTrace, loggingLevel);
            var innerException = exception.InnerException;
            while (innerException != null)
            {
                LogAtLevel("Inner Exception:", loggingLevel);
                LogAtLevel(innerException.Message, loggingLevel);
                LogAtLevel(innerException.StackTrace, loggingLevel);
                innerException = innerException.InnerException;
            }
        }


        protected string ParseMonikers(string input)
        {
            input ??= string.Empty;
            var now = DateTime.Now;
            input = input.Replace("{y}", now.Year.ToString());
            input = input.Replace("{M}", now.Month.ToString("00"));
            input = input.Replace("{d}", now.Day.ToString("00"));
            input = input.Replace("{H}", now.Hour.ToString("00"));
            input = input.Replace("{m}", now.Minute.ToString("00"));
            input = input.Replace("{s}", now.Second.ToString("00"));
            input = input.Replace("{f}", now.Millisecond.ToString("000"));
            input = input.Replace("{w}", ((int)now.DayOfWeek).ToString());
            input = input.Replace("{W}", now.DayOfWeek.ToString());
            input = input.Replace("{L}", LoggingLevelName);
            input = input.Replace("{D}", now.ToString("yyyy/MM/dd"));
            input = input.Replace("{T}", now.ToString("HH:mm:ss"));
            input = input.Replace("{t}", now.ToString("HH:mm:ss.fff"));
            return input;
        }


        private static string GetLoggingLevelName(int loggingLevel)
        {
            return loggingLevel switch
            {
                var _ when loggingLevel >= (int)LoggingLevel.Force => "FORCE",
                var _ when loggingLevel >= (int)LoggingLevel.Fatal => "FATAL",
                var _ when loggingLevel >= (int)LoggingLevel.Error => "ERROR",
                var _ when loggingLevel >= (int)LoggingLevel.Warning => "WARN",
                var _ when loggingLevel >= (int)LoggingLevel.Info => "INFO",
                var _ when loggingLevel >= (int)LoggingLevel.Verbose => "VERB",
                var _ when loggingLevel >= (int)LoggingLevel.Debug => "DEBUG",
                var _ when loggingLevel >= (int)LoggingLevel.Trace => "TRACE",
                _ => throw new UnrecognizedLoggingLevelException(loggingLevel),
            };
        }


        private string GetCallingMethod()
        {
            string[] stackTrace = Environment.StackTrace.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string methodToSearch = $"Scrybe.Loggers.ScrybeLoggerBase.GetCallingMethod()";
            int i = 0;
            while (!stackTrace[i].Contains(methodToSearch))
            {
                i++;
            }
            string callMethod = stackTrace[i + 3];
            int methodOpen = callMethod.IndexOf("(");
            callMethod = callMethod.Substring(6, methodOpen - 6);
            return callMethod;
        }
    }
}
