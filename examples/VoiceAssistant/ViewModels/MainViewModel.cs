using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NAudio.Wave;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VoiceAssistant.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        //private readonly SpeakatoRecognizer speakatoRecognizer;
        public MainViewModel()
        {
            CanListen = true;
            RecognizedVoice = "Say something!";
            //this.speakatoRecognizer = speakatoRecognizer;

            ListenCommand = new RelayCommand(() => Task.Run(Listen).Wait(), () => CanListen);
            CloseApplicationCommand = new RelayCommand(CloseApplication);
        }

        public ICommand ListenCommand { get; private set; }

        public async Task Listen()
        {
            //var capture = new WasapiLoopbackCapture();
            //var outStream = new MemoryStream();

            //var writer = new WaveFileWriter(outStream, capture.WaveFormat);
            //capture.DataAvailable += (s, a) =>
            //{
            //    writer.Write(a.Buffer, 0, a.BytesRecorded);
            //    if (writer.Position > capture.WaveFormat.AverageBytesPerSecond * 4)
            //    {
            //        capture.StopRecording();
            //    }
            //};

            //capture.RecordingStopped += (s, a) =>
            //{
            //    writer.Dispose();
            //    writer = null;
            //    capture.Dispose();
            //};

            //capture.StartRecording();
            //while (capture.CaptureState != NAudio.CoreAudioApi.CaptureState.Stopped)
            //{
            //    Thread.Sleep(500);
            //}
            //var text = await speakatoRecognizer.SpeechToText(outStream);
            RecognizedVoice = "Test";
        }

        public ICommand CloseApplicationCommand { get; private set; }

        public void CloseApplication()
        {
            this.Cleanup();
            System.Windows.Application.Current.Shutdown();
        }

        private string recognizedVoice;
        public string RecognizedVoice
        {
            get
            {
                return recognizedVoice;
            }
            private set
            {
                recognizedVoice = value;
                this.RaisePropertyChanged(nameof(RecognizedVoice));
            }
        }

        private bool canListen;
        public bool CanListen
        {
            get
            {
                return canListen;
            }
            private set
            {
                canListen = value;
                this.RaisePropertyChanged(nameof(CanListen));
            }
        }
    }
}
