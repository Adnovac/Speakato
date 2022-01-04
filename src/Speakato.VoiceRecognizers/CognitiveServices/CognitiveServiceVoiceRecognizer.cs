using Speakato.Abstractions;
using Speakato.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Speakato.VoiceRecognizer.CognitiveServices
{
    public class CognitiveServiceVoiceRecognizer : IVoiceRecognizerService
    {
        private readonly HttpClient httpClient;
        private readonly string language;
        public CognitiveServiceVoiceRecognizer(CognitiveServiceConfiguration configuration, HttpClient httpClient, string language)
        {
            httpClient.BaseAddress = configuration.Url;
            this.language = language;
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", configuration.Key);
            this.httpClient = httpClient;
        }

        public async Task<string> SpeechRecognizeAsync(Stream voiceStream)
        {
            StreamContent content = new StreamContent(voiceStream);
            HttpResponseMessage response = await httpClient.PostAsync($"?language={language}", content);
            string responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                ResponseModel result = JsonConvert.DeserializeObject<ResponseModel>(responseContent);
                return result.DisplayText;
            }
            throw new VoiceRecognizerServiceException(responseContent, response.StatusCode);
        }
    }
}
