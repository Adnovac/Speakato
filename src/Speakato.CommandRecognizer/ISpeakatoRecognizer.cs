using System.IO;
using System.Threading.Tasks;

namespace Speakato.CommandRecognizer
{
    public interface ISpeakatoRecognizer
    {
        Task<string> SpeechToCommand(Stream stream);
        Task<string> SpeechToText(Stream stream);
        string TextToCommand(string sentence);
        float[] TextToVector(string sentence);
    }
}
