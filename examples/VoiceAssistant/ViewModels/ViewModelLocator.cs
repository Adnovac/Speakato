using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using Speakato.CommandRecognizer;
using Speakato.Models;
using System;
using System.Net.Http;

namespace VoiceAssistant.ViewModels
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
                Url = new Uri(Environment.GetEnvironmentVariable("CognitiveServiceUrl")),
                ModelPath = modelPath,
                PythonEnvironmentPath = envPath
            };

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>(() => new MainViewModel(new SpeakatoRecognizer(new HttpClient(), config)));
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
    }
}
