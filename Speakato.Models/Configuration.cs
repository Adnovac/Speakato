using System;

namespace Speakato.Models
{
    public class GoogleCloudConfiguration : Configuration
    {
        //TODO: add google Config
    }

    public class CognitiveServiceConfiguration : Configuration
    {
        public string Key { get; set; }
        public Uri Url { get; set; }
    }

    /// <summary>
    /// Core configuration for Speakato library
    /// </summary>
    public abstract class Configuration
    {
        /// <summary>
        /// Language on which model is working on and Voice Recognizer will be working on.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Path to the directory produced by SpeakatoTrainer
        /// </summary>
        public string ModelPath { get; set; }
    }
}
