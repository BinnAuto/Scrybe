using System.Diagnostics;
using System.Text.Json.Nodes;

namespace Scrybe.Loggers
{
    internal class ScrybeEventViewerLogger : ScrybeLoggerBase
    {
        private readonly string Source;

        public ScrybeEventViewerLogger(LoggingLevel loggingLevel, JsonNode config, Type t)
            : base(loggingLevel, config)
        {
            Source = config["Source"]?.ToString() ?? t.ToString()!;
        }


        protected override void LogMessage(object? message)
        {
            string msgString = $"{message}";
            using(EventLog log = new("Application"))
            {
                log.Source = Source;
                switch(LoggingLevelName)
                {
                    case "FORCE":
                    case "INFO":
                    case "TRACE":
                    case "VERB":
                    case "DEBUG":
                        log.WriteEntry(msgString, EventLogEntryType.Information);
                        break;

                    case "WARN":
                        log.WriteEntry(msgString, EventLogEntryType.Warning);
                        break;

                    case "ERROR":
                    case "FATAL":
                        log.WriteEntry(msgString, EventLogEntryType.Error);
                        break;
                }
            }
        }
    }
}
