namespace Scrybe
{
    internal class CannotLoadConfigurationException : Exception
    {
        internal CannotLoadConfigurationException(string filePath, Exception ex)
            : base($"Configuration file \"{filePath}\" cannot be loaded. Check inner exception for more information", ex)
        { }
    }


    internal class InvalidLoggingLevelNameException : Exception
    {
        public InvalidLoggingLevelNameException(string? loggingLevelName)
            : base($"The provided logging level \"{loggingLevelName}\" is not recognized")
        { }
    }


    internal class UnrecognizedLoggingLevelException : Exception
    {
        public UnrecognizedLoggingLevelException(int loggingLevel)
            : base($"The provided logging level {loggingLevel} is not recognized")
        { }
    }


    internal class MissingConfigurationException : Exception
    {
        public MissingConfigurationException(string loggerType, string configurationName)
            : base($"The configuration field \"{configurationName}\" is required for {loggerType} loggers")
        { }
    }


    internal class InvalidLoggerTypeException : Exception
    {
        public InvalidLoggerTypeException(string loggerType)
            : base($"The provided logger type \"{loggerType}\" is not recognized")
        { }
    }
}
