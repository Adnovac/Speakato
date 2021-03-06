using System;

namespace Speakato.Models
{
    /// <summary>
    /// Configuration of Speakato in case of using Google Cloud
    /// </summary>
    public class GoogleCloudConfiguration : Configuration {}

    /// <summary>
    /// Configuration of Speakato in case of using Cognitive Services
    /// </summary>
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
        /// Path to the model produced by SpeakatoTrainer
        /// </summary>
        public string ModelPath { get; set; }

        /// <summary>
        /// Path to the virtual environment with the modules specified in SpeakatoTrainer installed
        /// </summary>
        public string PythonEnvironmentPath { get; set; }
    }
}
