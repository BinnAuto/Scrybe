using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Scrybe.Loggers
{
    internal static class ScrybeLoggerBuilder
    {
        internal static ScrybeLoggerBase[] GetScrybeLoggers<T>()
        {
            var loggers = ScrybeConfig.LoggerConfig;
            if(loggers == null)
            {
                return Array.Empty<ScrybeLoggerBase>();
            }

            List<ScrybeLoggerBase> resultList = new();
            bool stopScanning = false;
            string className = typeof(T).FullName!.ToUpper();
            foreach (var config in loggers)
            {
                if(stopScanning)
                {
                    break;
                }

                if (config == null)
                {
                    continue;
                }

                string classNamePattern = config["ClassNamePattern"]?.ToString()!;
                if (string.IsNullOrEmpty(classNamePattern))
                {
                    continue;
                }

                string classNamePatternRegex = classNamePattern.ToUpper().Replace(".", "[.]").Replace("*", ".*");
                Regex regex = new(classNamePatternRegex);
                if (!regex.IsMatch(className))
                {
                    continue;
                }

                stopScanning = string.Equals(config["StopScanning"]?.ToString(), "true", StringComparison.InvariantCultureIgnoreCase);
                var loggersConfig = config["Loggers"]?.AsArray();
                if(loggersConfig == null)
                {
                    continue;
                }

                var defaultLoggingLevel = GetDefaultLoggingLevel(config);
                foreach (var loggerConfig in loggersConfig)
                {
                    var logger = BuildLogger<T>(loggerConfig!, defaultLoggingLevel);
                    resultList.Add(logger);
                    logger.OnLoggerInitialized();
                }
            }
            var result = resultList.ToArray();
            return result;
        }


        private static LoggingLevel GetDefaultLoggingLevel(JsonNode loggerConfig)
        {
            var result = LoggingLevel.Error;
            try
            {
                string? defaultLogLevel = loggerConfig["DefaultLoggingLevel"]?.ToString();
                result = GetLoggingLevel(defaultLogLevel);
            }
            catch { }
            return result;
        }


        private static ScrybeLoggerBase BuildLogger<T>(JsonNode loggerConfig, LoggingLevel defaultLoggingLevel)
        {
            var loggingLevel = defaultLoggingLevel;
            string? configLoggingLevel = loggerConfig["LoggingLevel"]?.ToString();
            if (configLoggingLevel != null)
            {
                loggingLevel = GetLoggingLevel(configLoggingLevel);
            }

            string loggerType = loggerConfig["Type"]?.ToString() ?? string.Empty;
            switch(loggerType.ToUpper())
            {
                case "FILE":
                case "FILELOGGER":
                case "SCRYBEFILELOGGER":
                    return new ScrybeFileLogger(loggingLevel, loggerConfig);

                case "CONSOLE":
                case "CONSOLELOGGER":
                case "SCRYBECONSOLELOGGER":
                    return new ScrybeConsoleLogger(loggingLevel, loggerConfig);

                case "EVENTVIEWER":
                case "EVENTVIEWERLOGGER":
                case "SCRYBEEVENTVIEWERLOGGER":
                    return new ScrybeEventViewerLogger(loggingLevel, loggerConfig, typeof(T));

                default:
                    throw new InvalidLoggerTypeException(loggerType);
            }
        }

        private static LoggingLevel GetLoggingLevel(string? loggingLevelName)
        {
            string loggingLevelNameUpper = loggingLevelName?.ToUpper() ?? string.Empty;
            return loggingLevelNameUpper switch
            {
                "A" or "ALL" or "T" or "TRACE" => LoggingLevel.Trace,
                "V" or "VERBOSE" => LoggingLevel.Verbose,
                "D" or "DEBUG" => LoggingLevel.Debug,
                "I" or "INFO" or "INFORMATION" => LoggingLevel.Info,
                "W" or "WARN" or "WARNING" => LoggingLevel.Warning,
                "E" or "ERR" or "ERROR" => LoggingLevel.Error,
                "F" or "FATAL" => LoggingLevel.Fatal,
                "N" or "NONE" => LoggingLevel.None,
                _ => throw new InvalidLoggingLevelNameException(loggingLevelName),
            };
        }
    }
}
