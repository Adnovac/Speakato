using Newtonsoft.Json;

namespace Speakato.Models
{
    public class Settings
    {
        /// <summary>
        /// Language on which model is working on and Voice Recognizer will be working on.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Spacy default model
        /// </summary>
        [JsonProperty("spacy_model")]
        public string SpacyModel { get; set; }

        /// <summary>
        /// A length of a 
        /// </summary>
        [JsonProperty("token_len")]
        public string TokenLength { get; set; }
    }
}
