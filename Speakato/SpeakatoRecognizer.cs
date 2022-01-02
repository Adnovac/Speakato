using Microsoft.ML;
using Microsoft.ML.Transforms.Onnx;
using Newtonsoft.Json;
using Python.Runtime;
using SpacyDotNet;
using Speakato.Abstractions;
using Speakato.Models;
using Speakato.VoiceRecognizer.CognitiveServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Speakato
{
    public class SpeakatoRecognizer : ISpeakatoRecognizer
    {
        private readonly IVoiceRecognizerService voiceRecognizer;
        private readonly List<string> availableCommands;
        private readonly Settings settings;
        private readonly OnnxScoringEstimator estimator;
        private readonly Lang nlp;

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
            Environment.SetEnvironmentVariable("PYTHONHOME", configuration.PythonEnvironmentPath, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONPATH", $"{configuration.PythonEnvironmentPath}\\Lib\\site-packages;{configuration.PythonEnvironmentPath}\\Lib", EnvironmentVariableTarget.Process);

            Runtime.PythonDLL = $"python38.dll";
            PythonEngine.PythonHome = configuration.PythonEnvironmentPath;
            PythonEngine.PythonPath = PythonEngine.PythonPath + ";" + Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);

            MLContext mlContext = new MLContext();
            estimator = mlContext.Transforms.ApplyOnnxModel($"{configuration.ModelPath}/model.onnx");
            nlp = new Spacy().Load(settings.SpacyModel);
            availableCommands = new List<string>(File.ReadAllLines($"{configuration.ModelPath}/commands.txt"));
        }

        public string SpeechToCommand()
        {
            throw new NotImplementedException();
        }

        public string SpeechToText()
        {
            throw new NotImplementedException();
        }

        public string TextToCommand(string sentence)
        {
            var vector = TextToVector(sentence);
            throw new NotImplementedException();
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
