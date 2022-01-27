using GalaSoft.MvvmLight.Ioc;
using Speakato.CommandRecognizer;
using Speakato.Models;
using System;
using System.Net.Http;

namespace SpeakatoVoiceAssistant.ViewModels
{
    internal class ViewModelLocator
    {
        private const string modelPath = @"C:\dev\SpeakatoTrainer\models\spkt-test";
        private const string envPath = @"C:\Users\annad\anaconda3\envs\spkt";

        public ViewModelLocator()
        {
            var config = new CognitiveServiceConfiguration()
            {
                Key = Environment.GetEnvironmentVariable("CognitiveServiceKey"),
                Url = new Uri(Environment.GetEnvironmentVariable("CognitiveServiceUrl")!),
                ModelPath = modelPath,
                PythonEnvironmentPath = envPath
            };

            //TODO: change to false
            SpeakatoRecognizer recognizer = new SpeakatoRecognizer(new HttpClient(), config, true);
            SimpleIoc.Default.Register<MainViewModel>(() => new MainViewModel(recognizer));
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return SimpleIoc.Default.GetInstance<MainViewModel>();
            }
        }
    }
}
