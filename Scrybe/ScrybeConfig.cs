using System.Text.Json.Nodes;

namespace Scrybe
{
    internal static class ScrybeConfig
    {
        internal static bool IsIntialized { get; set; }

        private static JsonNode? ConfigJson;

        internal static void Initialize()
        {
            if(IsIntialized)
            {
                return;
            }

            LoadConfigurationFile();
            IsIntialized = true;
        }


        public static JsonArray? LoggerConfig
        {
            get
            {
                Initialize();
                return ConfigJson![nameof(LoggerConfig)]?.AsArray();
            }
        }


        private static void LoadConfigurationFile()
        {
            string filePath = "./ScrybeConfig.json";
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Scrybe Configuration file not found", filePath);
                }

                string jsonFile = File.ReadAllText(filePath);
                ConfigJson = JsonNode.Parse(jsonFile)!;
            }
            catch (Exception e)
            {
                throw new CannotLoadConfigurationException(filePath, e);
            }
        }
    }
}
