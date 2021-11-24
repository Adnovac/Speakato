using Speakato.Abstractions;
using Speakato.Models;
using Speakato.VoiceRecognizer.CognitiveServices;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Speakato
{
    public class Speakato : ISpeakato
    {
        private readonly IVoiceRecognizerService voiceRecognizer;
        private readonly List<string> availableCommands;
        public Speakato(HttpClient httpClient, GoogleCloudConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public Speakato(HttpClient httpClient, CognitiveServiceConfiguration configuration)
        {
            voiceRecognizer = new CognitiveServiceVoiceRecognizer(configuration, httpClient);
        }

        public string SpeechToCommand()
        {
            throw new NotImplementedException();
        }

        public string SpeechToText()
        {
            throw new NotImplementedException();
        }

        public float[,] SpeechToVector()
        {
            throw new NotImplementedException();
        }

        private void LoadCommands(string modelName)
        {
            throw new NotImplementedException();
        }
    }
}
