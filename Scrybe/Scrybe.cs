using Scrybe.Loggers;

namespace Scrybe
{
    public interface IScrybe<T>
    {
        /// <summary>
        /// Log a message at a user-defined logging level
        /// </summary>
        public void LogCustom(object? message, int loggingLevel);

        /// <summary>
        /// Log a message at the DEBUG logging level
        /// </summary>
        public void LogDebug(object? message);

        /// <summary>
        /// Log a message at the ERROR logging level
        /// </summary>
        public void LogError(object? message);

        /// <summary>
        /// Log a message and exception at the ERROR logging level
        /// </summary>
        public void LogError(object? message, Exception exception);

        /// <summary>
        /// Log a message at the FATAL logging level
        /// </summary>
        public void LogFatal(object? message);

        /// <summary>
        /// Log a message and exception at the FATAL logging level
        /// </summary>
        public void LogFatal(object? message, Exception exception);

        /// <summary>
        /// Log a message at the INFO logging level
        /// </summary>
        public void LogInfo(object? message);

        /// <summary>
        /// Log a message at the TRACE logging level
        /// </summary>
        public void LogTrace(object? message);

        /// <summary>
        /// Log a message at the VERBOSE logging level
        /// </summary>
        public void LogVerbose(object? message);

        /// <summary>
        /// Log a message at the WARNING logging level
        /// </summary>
        public void LogWarning(object? message);

        /// <summary>
        /// Log the start of a method and parameter values at the TRACE logging level
        /// </summary>
        public void LogMethodStart(params object?[] parameters);

        /// <summary>
        /// Log the end of a method at the TRACE logging level
        /// </summary>
        public void LogMethodEnd();

        /// <summary>
        /// Log the end of a method and the value of the resulting object at the TRACE level
        /// </summary>
        /// <param name="returnObject">The object returned by the calling method.</param>
        /// <returns>The object passed into this method</returns>
        public U? LogMethodEnd<U>(U? returnObject);

        /// <summary>
        /// Log an exception in the current method at the ERROR level
        /// </summary>
        public void LogErrorInMethod(Exception exception);

        /// <summary>
        /// Log a variable name and its value at the DEBUG level
        /// </summary>
        public void LogVariableValue(string variableName, object? variableValue);

        /// <summary>
        /// Log the time elapsed from a given start time at the INFO level
        /// </summary>
        public void LogTimeElapsed(DateTime startTime);
    }


    public class Scrybe<T> : IScrybe<T>
    {
        internal readonly ScrybeLoggerBase[] Loggers;

        public Scrybe()
        {
            Loggers = ScrybeLoggerBuilder.GetScrybeLoggers<T>();
        }

        public void LogCustom(object? message, int loggingLevel)
        {
            foreach (var logger in Loggers)
            {
                logger.LogCustom(message, loggingLevel);
            }
        }

        public void LogDebug(object? message)
        {
            foreach (var logger in Loggers)
            {
                logger.LogDebug(message);
            }
        }

        public void LogError(object? message)
        {
            foreach (var logger in Loggers)
            {
                logger.LogError(message);
            }
        }

        public void LogError(object? message, Exception exception)
        {
            foreach (var logger in Loggers)
            {
                logger.LogError(message, exception);
            }
        }

        public void LogFatal(object? message)
        {
            foreach (var logger in Loggers)
            {
                logger.LogFatal(message);
            }
        }

        public void LogFatal(object? message, Exception exception)
        {
            foreach (var logger in Loggers)
            {
                logger.LogFatal(message, exception);
            }
        }

        public void LogInfo(object? message)
        {
            foreach (var logger in Loggers)
            {
                logger.LogInfo(message);
            }
        }

        public void LogTrace(object? message)
        {
            foreach (var logger in Loggers)
            {
                logger.LogTrace(message);
            }
        }

        public void LogVerbose(object? message)
        {
            foreach (var logger in Loggers)
            {
                logger.LogVerbose(message);
            }
        }

        public void LogWarning(object? message)
        {
            foreach (var logger in Loggers)
            {
                logger.LogWarning(message);
            }
        }


        public void LogMethodStart(params object?[] parameters)
        {
            foreach (var logger in Loggers)
            {
                logger.LogMethodStart(parameters);
            }
        }


        public void LogMethodEnd()
        {
            foreach (var logger in Loggers)
            {
                logger.LogMethodEnd();
            }
        }


        public U? LogMethodEnd<U>(U? returnObject)
        {
            foreach (var logger in Loggers)
            {
                logger.LogMethodEnd(returnObject);
            }
            return returnObject;
        }


        public void LogErrorInMethod(Exception exception)
        {
            foreach (var logger in Loggers)
            {
                logger.LogErrorInMethod(exception);
            }
        }


        public void LogVariableValue(string variableName, object? variableValue)
        {
            foreach (var logger in Loggers)
            {
                logger.LogVariableValue(variableName, variableValue);
            }
        }


        public void LogTimeElapsed(DateTime startTime)
        {
            foreach (var logger in Loggers)
            {
                logger.LogTimeElapsed(startTime);
            }
        }
    }
}
