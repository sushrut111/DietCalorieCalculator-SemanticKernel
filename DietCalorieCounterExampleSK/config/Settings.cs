using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DietCalorieCounterExampleSK.config
{
    public static class Settings
    {
        public const string DefaultConfigFile = "config/settings.json";
        public const string ModelKey = "model";
        public const string EndpointKey = "endpoint";
        public const string SecretKey = "apikey";

        public static (string model, string azureEndpoint, string apiKey) LoadFromFile(string configFile = DefaultConfigFile)
        {
            if (!File.Exists(configFile))
            {
                Console.WriteLine("Configuration not found: " + configFile);
                throw new Exception($"Configuration not found! Please make sure the configFile: {configFile} exists!");
            }

            try
            {
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(configFile));
                string model = config[ModelKey];
                string azureEndpoint = config[EndpointKey];
                string apiKey = config[SecretKey];

                return (model, azureEndpoint, apiKey);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong: " + e.Message);
                return ("", "", "");
            }
        }
    }
}
