using Speakato.Abstractions;
using Speakato.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Speakato.VoiceRecognizer.Google
{
    public class GoogleCloudVoiceRecgonizer : IVoiceRecognizerService
    {
        public GoogleCloudVoiceRecgonizer(GoogleCloudConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public Task<string> SpeechRecognizeAsync(Stream voiceFileStream)
        {
            throw new NotImplementedException();
        }
    }
}
