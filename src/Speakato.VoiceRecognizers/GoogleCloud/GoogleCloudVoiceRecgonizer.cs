using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Speakato.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Speakato.VoiceRecognizers.Google
{
    public class GoogleCloudVoiceRecgonizer : IVoiceRecognizerService
    {
        private readonly SpeechClient speechClient;
        private readonly RecognitionConfig config;

        /// <summary>
        /// GOOGLE_APPLICATION_CREDENTIALS Has to be set before initializing this class
        /// </summary>
        public GoogleCloudVoiceRecgonizer(string language)
        {
            speechClient = SpeechClient.Create();
            string languageCode = null;

            if (language == "pl")
            {
                languageCode = LanguageCodes.Polish.Poland;
            }
            else if (language == "en")
            {
                languageCode = LanguageCodes.English.UnitedStates;
            }

            config = new RecognitionConfig
            {
                LanguageCode = languageCode
            };
        }

        /// <summary>
        /// Returns recognized speech from the given stream if possible.
        /// Returns null if speech isn't recognized. 
        /// </summary>
        /// <param name="voiceFileStream">Stream of a recording containing a sample with speech to be recognized (flac/wav format)</param>
        /// <returns>A string with a recognized speech</returns>
        public async Task<string> SpeechRecognizeAsync(Stream voiceFileStream)
        {
            RecognitionAudio audio = RecognitionAudio.FromStream(voiceFileStream);
            var response = await speechClient.RecognizeAsync(config, audio);
            var result = response.Results.FirstOrDefault();

            return result?.Alternatives.FirstOrDefault()?.Transcript;
        }
    }
}
