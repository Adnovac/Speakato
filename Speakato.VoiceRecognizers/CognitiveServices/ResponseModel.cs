namespace Speakato.VoiceRecognizer.CognitiveServices
{
    internal class ResponseModel
    {
        public string RecognitionStatus { get; set; }
        public string DisplayText { get; set; }
        public int Offset { get; set; }
        public int Duration { get; set; }
    }
}
