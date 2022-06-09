using Microsoft.ML;
using Newtonsoft.Json;
using Python.Runtime;
using SpacyDotNet;
using Speakato.CommandRecognizer.Models;
using Speakato.Models;
using Speakato.VoiceRecognizers;
using Speakato.VoiceRecognizers.CognitiveServices;
using Speakato.VoiceRecognizers.Google;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Speech.Recognition;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Speakato.CommandRecognizer
{
    public class SpeakatoRecognizer : ISpeakatoRecognizer
    {
        public readonly Settings settings;
        public delegate Task CommandDecetedEventHandler();
        public event CommandDecetedEventHandler CommandDetected;
        private readonly IVoiceRecognizerService voiceRecognizer;
        private Lang nlp;
        private List<string> availableCommands;
        private PredictionEngine<OnnxModelInput, OnnxModelOutput> predictionEngine;
        private SpeechRecognitionEngine recognizer;
        public SpeakatoRecognizer(GoogleCloudConfiguration configuration, bool loadModel = true) : this(configuration as Configuration, loadModel)
        {
            voiceRecognizer = new GoogleCloudVoiceRecgonizer(settings.Language);
        }

        public SpeakatoRecognizer(HttpClient httpClient, CognitiveServiceConfiguration configuration, bool loadModel = true) : this(configuration as Configuration, loadModel)
        {
            voiceRecognizer = new CognitiveServiceVoiceRecognizer(configuration, httpClient, settings.Language);
        }

        private SpeakatoRecognizer(Configuration configuration, bool loadModel = true)
        {
            var settingsContent = File.ReadAllText($"{configuration.ModelPath}/info.json");
            settings = JsonConvert.DeserializeObject<Settings>(settingsContent);
            settings.ModelPath = configuration.ModelPath;

            Environment.SetEnvironmentVariable("PATH", configuration.PythonEnvironmentPath, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONHOME", configuration.PythonEnvironmentPath, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONPATH", $"{configuration.PythonEnvironmentPath}\\Lib\\site-packages;{configuration.PythonEnvironmentPath}\\Lib", EnvironmentVariableTarget.Process);

            Runtime.PythonDLL = $"python38.dll";
            PythonEngine.PythonHome = configuration.PythonEnvironmentPath;
            PythonEngine.PythonPath = PythonEngine.PythonPath + ";" + Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);

            if (loadModel) LoadModel();
        }

        /// <summary>
        /// Loads a model. Call it before first use of any other method if loadModel was set to false in the constructor.
        /// </summary>
        public void LoadModel()
        {
            nlp = new Spacy().Load(settings.SpacyModel);

            MLContext mlContext = new MLContext();

            var onnxPredictionPipeline = mlContext
                                            .Transforms
                                            .ApplyOnnxModel(
                                                outputColumnNames: new string[] { "dense_3" },
                                                inputColumnNames: new string[] { "dense_input" },
                                                $"{settings.ModelPath}/model.onnx");

            var predictionPipeline = onnxPredictionPipeline.Fit(mlContext.Data.LoadFromEnumerable(new OnnxModelInput[] { }));
            predictionEngine = mlContext.Model.CreatePredictionEngine<OnnxModelInput, OnnxModelOutput>(predictionPipeline);
            availableCommands = new List<string>(File.ReadAllLines($"{settings.ModelPath}/commands.txt"));
        }

        /// <summary>
        /// Predicts command with a model trained in SpeakatoTrainer 
        /// Returns null if command isn't recognized.
        /// </summary>
        /// <param name="stream">Stream of a recording containing a sample with speech to be recognized</param>
        /// <returns>Recognized command</returns>
        public async Task<string> SpeechToCommand(Stream stream)
        {
            var text = await voiceRecognizer.SpeechRecognizeAsync(stream);
            return TextToCommand(text);
        }

        /// <summary>
        /// Returns recognized speech from the given stream if possible.
        /// Returns null if speech isn't recognized. 
        /// </summary>
        /// <param name="stream">Stream of a recording containing a sample with speech to be recognized</param>
        /// <returns>A string with a recognized speech</returns>
        public async Task<string> SpeechToText(Stream stream)
        {
            var text = await voiceRecognizer.SpeechRecognizeAsync(stream);
            return text;
        }

        /// <summary>
        /// Predicts command with a model trained in SpeakatoTrainer 
        /// Returns null if command isn't recognized.
        /// </summary>
        /// <param name="sentence">Sentence from which a command should be recognized</param>
        /// <returns>Recognized command</returns>
        public string TextToCommand(string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence)) return null;
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

        /// <summary>
        /// Looks for orgName, date, placeName, and other labels depending on a selected language. 
        /// Returns list of tuples as <text, recognized label>
        /// </summary>
        /// <param name="sentence">Sentence from which labels should be recognized</param>
        /// <returns>List of tuples as <text, recognized label></text></returns>
        public List<(string, string)> GetEnts(string sentence)
        {
            List<(string, string)> ents = new List<(string, string)>();
            var document = nlp.GetDocument(sentence);
            foreach (var span in document.Ents)
            {
                ents.Add((span.Text, span.Label));
            }
            return ents;
        }

        /// <summary>
        /// Turns on continues listening by SpeechRecognitionEngine. When desired activationCommand is detected CommandDetected event will be invoked.
        /// </summary>
        /// <param name="activationCommand">Command which will be recognized by SpeechRecognitionEngine. It has to be an english word.</param>
        public void ToggleListening(string activationCommand = "start")
        {
            if (recognizer == null)
            {
                recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
                var grammar = new Grammar(new GrammarBuilder(new Choices(new string[] { activationCommand })));
                recognizer.LoadGrammar(grammar);
                recognizer.SetInputToDefaultAudioDevice();
                recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            }
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            CommandDetected?.Invoke();
        }

        private float[] TextToVector(string sentence)
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
