using DietCalorieCounterExampleSK.config;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DietCalorieCounterExampleSK
{
    public class PluginsConsumer
    {
        private Kernel kernel {  get; set; }
        private KernelPlugin plugins { get; set; }

        private const string User = "User";
        private const string Assistant = "Assistant";
        private const string CaloriePluginName = "calories";

        public PluginsConsumer() 
        {
            var (model, endpoint, apiKey) = Settings.LoadFromFile();

            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(model, endpoint, apiKey);
            this.kernel = builder.Build();

            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string absDirectoryPath = Path.Join(basePath, "plugins");

            var plugins = this.kernel.ImportPluginFromPromptDirectory(absDirectoryPath);
            this.plugins = plugins;
        }

        public async Task<FunctionResult> UseCaloriePlugin(string userMessage, string history)
        {
            var plugin = this.plugins[CaloriePluginName];

            if (plugin == null)
            {
                throw new Exception($"Plugin {CaloriePluginName} was not found!");
            }

            KernelArguments args = new()
            {
                ["inputString"] = userMessage,
                ["history"] = history
            };

            return await this.kernel.InvokeAsync(plugin, args);
        }

        public async Task LaunchChat()
        {
            string history = "";
            while (true)
            {
                Console.Write($"{User} > ");
                string userMessage = Console.ReadLine() ?? string.Empty;
                history = AddMessage(history, userMessage, User);

                var result = await this.UseCaloriePlugin(userMessage, history);
                history = AddMessage(history, result.ToString(), Assistant);

                Console.WriteLine($"{Assistant} > " + result);
            }
        }

        private string AddMessage(string history, string message, string role)
        {
            history = history + $"{role}: {message}\n";
            return history;
        }
    }
}
