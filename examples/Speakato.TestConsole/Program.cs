using Microsoft.Extensions.Configuration;
using Speakato.CommandRecognizer;
using Speakato.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Speakato.TestConsole
{
    class Program
    {
        private static readonly ISpeakatoRecognizer speakatoRecognizer;
        private const string modelPath = @"C:\dev\SpeakatoTrainer\models\spkt-test";
        private const string envPath = @"C:\Users\annad\anaconda3\envs\spkt";
        private const string clipsPath = @"C:\Users\annad\Documents\Sound recordings";
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

            speakatoRecognizer = new SpeakatoRecognizer(new HttpClient(), cognitiveConfig);
        }

        static async Task Main()
        {
            //foreach (string filePath in Directory.GetFiles(clipsPath))
            //{
            //    try
            //    {
            //        Stream inputStream = File.OpenRead(filePath);
            //        string result = await speakatoRecognizer.SpeechToText(inputStream);
            //        Console.WriteLine(result);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //}
            //var command = speakatoRecognizer.TextToCommand("Siema, co tam?");
            foreach (var ent in speakatoRecognizer.GetEnts("SIema, idę dzisiaj do Paryża do restauracji na 20:00 w lutym i w Google o piątej trzydzieści"))
                Console.WriteLine($"{ent.Item1} : {ent.Item2}");
        }
    }
}