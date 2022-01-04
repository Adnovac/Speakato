using Microsoft.ML;
using Newtonsoft.Json;
using Python.Runtime;
using SpacyDotNet;
using Speakato.Abstractions;
using Speakato.CommandRecognizer.Models;
using Speakato.Models;
using Speakato.VoiceRecognizer.CognitiveServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Speakato.CommandRecognizer
{
    public class SpeakatoRecognizer : ISpeakatoRecognizer
    {
        private readonly IVoiceRecognizerService voiceRecognizer;
        private readonly List<string> availableCommands;
        private readonly Settings settings;
        private readonly Lang nlp;
        private readonly PredictionEngine<OnnxModelInput, OnnxModelOutput> predictionEngine;

        public SpeakatoRecognizer(HttpClient httpClient, GoogleCloudConfiguration configuration) : this(configuration as Configuration)
        {
            throw new NotImplementedException();
        }

        public SpeakatoRecognizer(HttpClient httpClient, CognitiveServiceConfiguration configuration) : this(configuration as Configuration)
        {
            voiceRecognizer = new CognitiveServiceVoiceRecognizer(configuration, httpClient, settings.Language);
        }

        private SpeakatoRecognizer(Configuration configuration)
        {
            var settingsContent = File.ReadAllText($"{configuration.ModelPath}/info.json");
            settings = JsonConvert.DeserializeObject<Settings>(settingsContent);

            Environment.SetEnvironmentVariable("PATH", configuration.PythonEnvironmentPath, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONPATH", $"{configuration.PythonEnvironmentPath}\\Lib\\site-packages;{configuration.PythonEnvironmentPath}\\Lib", EnvironmentVariableTarget.Process);

            Runtime.PythonDLL = $"python38.dll";
            PythonEngine.PythonHome = configuration.PythonEnvironmentPath;
            PythonEngine.PythonPath = PythonEngine.PythonPath + ";" + Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);

            nlp = new Spacy().Load(settings.SpacyModel);

            MLContext mlContext = new MLContext();

            var onnxPredictionPipeline = mlContext
                                            .Transforms
                                            .ApplyOnnxModel(
                                                outputColumnNames: new string[] { "dense_3" },
                                                inputColumnNames: new string[] { "dense_input" },
                                                $"{configuration.ModelPath}/model.onnx");

            var predictionPipeline = onnxPredictionPipeline.Fit(mlContext.Data.LoadFromEnumerable(new OnnxModelInput[] { }));
            predictionEngine = mlContext.Model.CreatePredictionEngine<OnnxModelInput, OnnxModelOutput>(predictionPipeline);
            availableCommands = new List<string>(File.ReadAllLines($"{configuration.ModelPath}/commands.txt"));
        }

        public async Task<string> SpeechToCommand(Stream stream)
        {
            var text = await voiceRecognizer.SpeechRecognizeAsync(stream);
            return TextToCommand(text);
        }

        public async Task<string> SpeechToText(Stream stream)
        {
            var text = await voiceRecognizer.SpeechRecognizeAsync(stream);
            return text;
        }

        public string TextToCommand(string sentence)
        {
            var vector = TextToVector(sentence);
            var modelInput = new OnnxModelInput { Vector = vector };
            var predictedFare = predictionEngine.Predict(modelInput).PredictedFare;
            if (predictedFare.Max() > 0.5f)
            {
                var index = predictedFare.ToList().IndexOf(predictedFare.Max());
                return availableCommands[index];
            }
            return null;
        }

        public float[] TextToVector(string sentence)
        {
            var destSentence = CleanSentence(sentence);
            return nlp.GetDocument(destSentence.Trim()).GetVector.ToArray();
        }

        private string CleanSentence(string sentence)
        {
            var cleanedSentence = sentence.ToLower();
            var destSentence = string.Empty;

            cleanedSentence = Regex.Replace(cleanedSentence, @"!@#$%^&*(){}?/;`~:<>+=-],.", "");
            foreach (var word in nlp.GetDocument(cleanedSentence).Tokens)
            {
                if (!nlp.GetStopWords.Contains(word.Lemma)) destSentence += $"{word.Lemma} ";
            }
            return destSentence;
        }
    }
}
