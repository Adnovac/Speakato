using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NAudio.Wave;
using Speakato.CommandRecognizer;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeakatoVoiceAssistant.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private SpeakatoRecognizer? speakatoRecognizer;
        private CommandResolver commandResolver = new CommandResolver(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        public MainViewModel(SpeakatoRecognizer speakatoRecognizer)
        {
            this.speakatoRecognizer = speakatoRecognizer;
            ListenCommand = new RelayCommand(async () => await Listen(), () => CanListen);
            CanListen = false;

            RecognizedVoice = "Speakato is loading. Please wait...";

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(PrepareSpeakatoRecognizer);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((object? sender, RunWorkerCompletedEventArgs e) =>
                {
                    CanListen = true;
                }
            );
            backgroundWorker.RunWorkerAsync();
            CloseApplicationCommand = new RelayCommand(CloseApplication);
        }

        private void PrepareSpeakatoRecognizer(object? sender, DoWorkEventArgs e)
        {
            //TODO: Background model loading
            //speakatoRecognizer!.LoadModel();
            RecognizedVoice = "Say something!";
        }

        public ICommand ListenCommand { get; private set; }

        public async Task Listen()
        {
            CanListen = false;
            try
            {
                var content = await speakatoRecognizer!.SpeechToText(new MemoryStream(GetMicrophoneInput()));
                var commandRaw = speakatoRecognizer!.TextToCommand(content);
                RecognizedVoice = content;

                if (commandResolver != null)
                {
                    Command command = (Command)Enum.Parse(typeof(Command), commandRaw, true);
                    commandResolver.ResolveCommnad(command, content);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                CanListen = true;
            }
        }

        public ICommand CloseApplicationCommand { get; private set; }

        public void CloseApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private byte[] GetMicrophoneInput()
        {
            byte[] result;
            using (var waveIn = new NAudio.Wave.WaveInEvent
            {
                DeviceNumber = 0, //TODO: Microphone chooser
                WaveFormat = new NAudio.Wave.WaveFormat(rate: 44100, bits: 16, channels: 1),
                BufferMilliseconds = 50
            })
            using (var stream = new MemoryStream())
            using (var writer = new WaveFileWriter(stream, waveIn.WaveFormat))
            {
                waveIn.DataAvailable += (s, a) =>
                {
                    writer.Write(a.Buffer, 0, a.Buffer.Length);
                };
                waveIn.StartRecording();
                System.Threading.Thread.Sleep(3 * 1000);
                waveIn.StopRecording();
                writer.Flush();

                result = stream.ToArray();
            }
            return result;
        }

        private string recognizedVoice = string.Empty;
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
                ((RelayCommand)ListenCommand).RaiseCanExecuteChanged();
            }
        }
    }

}
