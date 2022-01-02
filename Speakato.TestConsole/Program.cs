using Microsoft.Extensions.Configuration;
using Speakato.Models;
using System;
using System.Net.Http;

namespace Speakato.TestConsole
{
    class Program
    {
        private static readonly ISpeakatoRecognizer speakatoRecognizer;
        private const string modelPath = @"C:\dev\SpeakatoTrainer\models\Speakato_model_2022-01-02";
        static Program()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration Configuration = builder.Build();

            builder.AddAzureKeyVault(Configuration.GetSection("KeyVault")["Url"]);
            Configuration = builder.Build();

            var config = new CognitiveServiceConfiguration()
            {
                Key = Configuration.GetSection("CognitiveService")["Key"],
                Url = new Uri(Configuration.GetSection("CognitiveService")["Url"]),
                ModelPath = modelPath,
                PythonEnvironmentPath = Configuration["PythonEnvironmentPath"]
            };

            speakatoRecognizer = new SpeakatoRecognizer(new HttpClient(), config);
        }

        static void Main()
        {
            speakatoRecognizer.TextToVector("Siema, co tam w nowym roku się dzieje ciekawego");
        }
    }
}