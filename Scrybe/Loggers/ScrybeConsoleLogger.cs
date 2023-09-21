using System.Text.Json.Nodes;

namespace Scrybe.Loggers
{
    internal class ScrybeConsoleLogger : ScrybeLoggerBase
    {
        private readonly string LogLinePrefix;

        public ScrybeConsoleLogger(LoggingLevel loggingLevel, JsonNode config)
            : base(loggingLevel, config)
        {
            LogLinePrefix = config["LogLinePrefix"]?.ToString() ?? string.Empty;
        }

        protected override void LogMessage(object? message)
        {
            message ??= string.Empty;
            string msgString = ParseMonikers(LogLinePrefix) + message.ToString();
            Console.WriteLine(msgString);
        }
    }
}
