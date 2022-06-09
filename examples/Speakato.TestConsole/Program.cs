using Microsoft.Extensions.Configuration;
using Speakato.CommandRecognizer;
using Speakato.Models;
using System;

namespace Speakato.TestConsole
{
    class Program
    {
        private static readonly ISpeakatoRecognizer speakatoRecognizer;
        private const string modelPath = @"C:\dev\SpeakatoTrainer\models\Speakato_model_2022-06-09";
        private const string envPath = @"C:\Users\annad\anaconda3\envs\spkt";
        static Program()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                                    .AddEnvironmentVariables();
            IConfiguration Configuration = builder.Build();

#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var cognitiveConfig = new CognitiveServiceConfiguration
            {
                Key = Configuration["CognitiveServiceKey"],
                Url = new Uri(Configuration["CognitiveServiceUrl"]),
                ModelPath = modelPath,
                PythonEnvironmentPath = envPath
            };
#pragma warning restore IDE0059 // Unnecessary assignment of a value


            var googleConfig = new GoogleCloudConfiguration
            {
                ModelPath = modelPath,
                PythonEnvironmentPath = envPath
            };

            speakatoRecognizer = new SpeakatoRecognizer(googleConfig, false);
        }

        static void Main()
        {
            speakatoRecognizer.LoadModel();
            speakatoRecognizer.ToggleListening();
            while (true)
            {

            }
        }
    }

    internal class Entity
    {
        public string Path { get; set; }
        public string Text { get; set; }
        public bool IsCommand { get; set; }
        public int Quality { get; set; }
        public bool? Recognized { get; set; }
        public string RecognizedText { get; set; }
    }
}