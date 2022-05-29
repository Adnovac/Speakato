using Microsoft.Extensions.Configuration;
using Speakato.CommandRecognizer;
using Speakato.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Speakato.TestConsole
{
    class Program
    {
        static void ToggleListening()
        {
            var recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            recognizer.LoadGrammar(new DictationGrammar());
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        static void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("Recognized text: " + e.Result.Text);
        }


        private static readonly ISpeakatoRecognizer speakatoRecognizer;
        private const string modelPath = @"C:\dev\SpeakatoTrainer\models\spkt-test";
        private const string envPath = @"C:\Users\annad\anaconda3\envs\spkt";
        static Program()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                                    .AddEnvironmentVariables();
            IConfiguration Configuration = builder.Build();

            var cognitiveConfig = new CognitiveServiceConfiguration
            {
                Key = Configuration["CognitiveServiceKey"],
                Url = new Uri(Configuration["CognitiveServiceUrl"]),
                ModelPath = modelPath,
                PythonEnvironmentPath = envPath
            };

            var googleConfig = new GoogleCloudConfiguration
            {
                ModelPath = modelPath,
                PythonEnvironmentPath = envPath
            };

            speakatoRecognizer = new SpeakatoRecognizer(googleConfig);
        }

        static async Task Main()
        {
            ToggleListening();
            while (true)
            {

            }
        }

        private static List<Entity> ReadCSV(string path)
        {
            var entities = new List<Entity>();
            foreach (var line in File.ReadAllLines(path).Skip(1))
            {
                var segments = line.Split(';');
                var entity = new Entity
                {
                    Path = segments[0],
                    Text = segments[1],
                    IsCommand = segments[2] == "TRUE",
                    Quality = int.Parse(segments[3]),
                    Recognized = (segments.Count() > 4 && segments[4] != null) ? segments[4] == "TRUE" : null,
                    RecognizedText = segments.Count() > 5 ? segments[5] : null
                };
                entities.Add(entity);
            }
            return entities;
        }

        private static void WriteCSV(string path, List<Entity> entities)
        {
            List<string> lines = new List<string> { "path;text;iscommand;quality;recognized;recognized_text" };
            foreach (var entity in entities)
            {
                lines.Add($"{entity.Path};{entity.Text};{entity.IsCommand};{entity.Quality};{entity.Recognized};{entity.RecognizedText}");
            }
            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        private static bool AreEquivalent(string item1, string item2)
        {
            item1 = Regex.Replace(item1.ToLower(), @"\p{P}", "");
            item2 = Regex.Replace(item2.ToLower(), @"\p{P}", "");

            return item1 == item2;
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