using Scrybe.Loggers;
using System.Text.Json.Nodes;

namespace Scrybe
{
    internal class ScrybeFileLogger : ScrybeLoggerBase
    {
        private readonly string LogLinePrefix;

        private readonly string? FilePath;

        private string? FileDirectory;

        private string? FileName;

        private const string? FileExtension = ".scrybe";

        public ScrybeFileLogger(LoggingLevel loggingLevel, JsonNode config)
            : base(loggingLevel, config)
        {
            LogLinePrefix = config["LogLinePrefix"]?.ToString() ?? string.Empty;
            FilePath = config["FilePath"]?.ToString();
            PrepareFile();

            int? maximumFileDays = (int?)config["MaxFileDays"];
            DeleteOldFiles(maximumFileDays);
        }

        private void PrepareFile()
        {
            if(string.IsNullOrEmpty(FilePath))
            {
                throw new MissingConfigurationException(GetType().Name, nameof(FilePath));
            }

            string filePath = Path.GetFullPath(FilePath);
            FileDirectory = Path.GetDirectoryName(filePath);
            FileName = Path.GetFileNameWithoutExtension(filePath);
        }


        protected override void LogMessage(object? message)
        {
            message ??= string.Empty;
            string msgString = ParseMonikers(LogLinePrefix) + message.ToString();
            string filePath = GetFilePath();
            bool logWritten = false;
            while(!logWritten)
            {
                try
                {
                    File.AppendAllLines(filePath, new string[] { msgString });
                    logWritten = true;
                }
                catch (IOException) { }
            }
        }


        private string GetFilePath()
        {
            Directory.CreateDirectory(FileDirectory!);
            string fileName = ParseMonikersSimple(FileName!);
            string result = Path.Combine(FileDirectory!, fileName + FileExtension);
            return result;
        }


        private void DeleteOldFiles(int? maximumFileDays)
        {
            if(!maximumFileDays.HasValue)
            {
                return;
            }

            var now = DateTime.Now;
            string[] files = Directory.GetFiles(FileDirectory!, $"*{FileExtension}", SearchOption.TopDirectoryOnly);
            foreach(var file in files)
            {
                FileInfo fileInfo = new(file);
                if(fileInfo.CreationTime > now.AddDays(-maximumFileDays.Value))
                {
                    continue;
                }

                int attempt = 1;
                bool deleted = false;
                while(!deleted)
                {
                    try
                    {
                        fileInfo.Delete();
                        deleted = true;
                    }
                    catch (IOException)
                    {
                        if (attempt == 10)
                        {
                            deleted = true;
                            throw;
                        }
                        attempt++;
                    }
                }
            }
        }


        private string ParseMonikersSimple(string input)
        {
            input ??= string.Empty;
            var now = DateTime.Now;
            input = input.Replace("{y}", now.Year.ToString());
            input = input.Replace("{M}", now.Month.ToString("00"));
            input = input.Replace("{d}", now.Day.ToString("00"));
            input = input.Replace("{H}", now.Hour.ToString("00"));
            return input;
        }
    }
}
