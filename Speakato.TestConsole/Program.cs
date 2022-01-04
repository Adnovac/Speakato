using Microsoft.Extensions.Configuration;
using Speakato.CommandRecognizer;
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

            builder.AddEnvironmentVariables();
            IConfiguration Configuration = builder.Build();

            var config = new CognitiveServiceConfiguration()
            {
                Key = Configuration["CognitiveServiceKey"],
                Url = new Uri(Configuration["CognitiveServiceUrl"]),
                ModelPath = modelPath,
                PythonEnvironmentPath = Configuration["PythonEnvironmentPath"]
            };

            speakatoRecognizer = new SpeakatoRecognizer(new HttpClient(), config);
        }

        static void Main()
        {
            var value = speakatoRecognizer.TextToCommand("Siema, co tam?");
            Console.WriteLine(value);
        }
    }
}