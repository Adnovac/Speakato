namespace Speakato
{
    public interface ISpeakatoRecognizer
    {
        string SpeechToText();
        string SpeechToCommand();
        float[] TextToVector(string sentence);
    }
}
