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
        public ViewModelLocator()
        {
            //var config = new CognitiveServiceConfiguration()
            //{
            //    Key = Environment.GetEnvironmentVariable("CognitiveServiceKey"),
            //    Url = new Uri(Environment.GetEnvironmentVariable("CognitiveServiceUrl")),
            //    ModelPath = modelPath,
            //    PythonEnvironmentPath = envPath
            //};

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>(() => new MainViewModel(null));
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
