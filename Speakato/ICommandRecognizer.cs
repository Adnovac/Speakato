namespace Speakato
{
    public interface ISpeakato
    {
        string SpeechToText();
        float[,] SpeechToVector();
        string SpeechToCommand();
    }
}
