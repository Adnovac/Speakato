using System.IO;
using System.Threading.Tasks;

namespace Speakato.VoiceRecognizers
{
    public interface IVoiceRecognizerService
    {
        /// <summary>
        /// Returns recognized speech from the given stream if possible.
        /// Returns null if speech isn't recognized. 
        /// </summary>
        /// <param name="voiceFileStream">Stream of a recording containing a sample with speech to be recognized</param>
        /// <returns>A string with a recognized speech</returns>
        Task<string> SpeechRecognizeAsync(Stream voiceFileStream);
    }
}