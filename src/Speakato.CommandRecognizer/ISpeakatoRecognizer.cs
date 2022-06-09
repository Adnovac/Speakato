using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Speakato.CommandRecognizer
{
    public interface ISpeakatoRecognizer
    {
        /// <summary>
        /// Predicts command with a model trained in SpeakatoTrainer 
        /// Returns null if command isn't recognized.
        /// </summary>
        /// <param name="stream">Stream of a recording containing a sample with speech to be recognized</param>
        /// <returns>Recognized command</returns>
        Task<string> SpeechToCommand(Stream stream);

        /// <summary>
        /// Returns recognized speech from the given stream if possible.
        /// Returns null if speech isn't recognized. 
        /// </summary>
        /// <param name="stream">Stream of a recording containing a sample with speech to be recognized</param>
        /// <returns>A string with a recognized speech</returns>
        Task<string> SpeechToText(Stream stream);

        /// <summary>
        /// Predicts command with a model trained in SpeakatoTrainer 
        /// Returns null if command isn't recognized.
        /// </summary>
        /// <param name="sentence">Sentence from which a command should be recognized</param>
        /// <returns>Recognized command</returns>
        string TextToCommand(string sentence);

        /// <summary>
        /// Looks for orgName, date, placeName, and other labels depending on a selected language. 
        /// Returns list of tuples as <text, recognized label>
        /// </summary>
        /// <param name="sentence">Sentence from which labels should be recognized</param>
        /// <returns>List of tuples as <text, recognized label></text></returns>
        List<(string, string)> GetEnts(string sentence);

        /// <summary>
        /// Loads a model. Call it before first use of any other method if loadModel was set to false in the constructor.
        /// </summary>
        void LoadModel();

        /// <summary>
        /// Turns on continues listening by SpeechRecognitionEngine. When desired activationCommand is detected CommandDetected event will be invoked.
        /// </summary>
        /// <param name="activationCommand">Command which will be recognized by SpeechRecognitionEngine. It has to be an english word.</param>
        void ToggleListening(string activationCommand = "start");
    }
}
