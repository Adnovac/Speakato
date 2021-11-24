using System.IO;
using System.Threading.Tasks;

namespace Speakato.Abstractions
{
    public interface IVoiceRecognizerService
    {
        Task<string> SpeechRecognizeAsync(Stream voiceFileStream);
    }
}