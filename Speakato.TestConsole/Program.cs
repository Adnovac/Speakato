using Speakato.Abstractions;
using Speakato.Models;
using Speakato.VoiceRecognizer.CognitiveServices;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Speakato.TestConsole
{
    class Program
    {
        private static readonly IVoiceRecognizerService voiceRecognizerService;
        private const string path = @"C:\Users\Ania\Desktop\Asystent głosowy - materiał\short_clips";
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
                Language = Configuration["Language"]
            };

            voiceRecognizerService = new CognitiveServiceVoiceRecognizer(config, new HttpClient());
        }

        static async Task Main()
        {
            foreach (string filePath in Directory.GetFiles(path))
            {
                try
                {
                    Stream inputStream = File.OpenRead(filePath);
                    string result = await voiceRecognizerService.SpeechRecognizeAsync(inputStream);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}