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
        private const string modelPath = @"C:\dev\SpeakatoTrainer\models\spkt-test";
        private const string envPath = @"C:\Users\annad\anaconda3\envs\spkt";
        static Program()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                                    .AddEnvironmentVariables();
            IConfiguration Configuration = builder.Build();

            var config = new CognitiveServiceConfiguration()
            {
                Key = Configuration["CognitiveServiceKey"],
                Url = new Uri(Configuration["CognitiveServiceUrl"]),
                ModelPath = modelPath,
                PythonEnvironmentPath = envPath
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