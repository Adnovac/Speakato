using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NAudio.Wave;
using Speakato.CommandRecognizer;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Reflection;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SpeakatoVoiceAssistant.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly SpeakatoRecognizer? speakatoRecognizer;
        private readonly CommandResolver commandResolver = new(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        private readonly SpeechSynthesizer synthesizer = new();
        private readonly SoundPlayer soundPlayer;

        public MainViewModel(SpeakatoRecognizer speakatoRecognizer)
        {
            var bubbleSoundPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "bubble_sound.wav");
            soundPlayer = new(bubbleSoundPath);
            soundPlayer.Load();
            this.speakatoRecognizer = speakatoRecognizer;
            this.speakatoRecognizer.CommandDetected += SpeakatoRecognizer_CommandDetected;

            if (speakatoRecognizer.settings.Language == "pl")
            {
                try
                {
                    synthesizer.SelectVoice("Microsoft Paulina Desktop");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            ListenCommand = new RelayCommand(async () => await Listen(), () => CanListen);
            CanListen = false;

            RecognizedVoice = "Wczytywanie speakato...";

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(PrepareSpeakatoRecognizer);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((object? sender, RunWorkerCompletedEventArgs e) =>
                {
                    CanListen = true;
                }
            );
            backgroundWorker.RunWorkerAsync();
            CloseApplicationCommand = new RelayCommand(CloseApplication);
            synthesizer.SetOutputToDefaultAudioDevice();
            this.speakatoRecognizer.ToggleListening();
        }

        private async Task SpeakatoRecognizer_CommandDetected()
        {
            Thread.Sleep(1000);
            await Listen();
        }

        private void PrepareSpeakatoRecognizer(object? sender, DoWorkEventArgs e)
        {
            RecognizedVoice = "Powiedz start aby nasłuchiwać albo wciśnij przycisk!";
        }

        public ICommand ListenCommand { get; private set; }


        public async Task Listen()
        {
            RecorderColor = "#750219";
            CanListen = false;
            soundPlayer.Play();
            try
            {
                var content = await speakatoRecognizer!.SpeechToText(new MemoryStream(GetMicrophoneInput()));
                RecognizedVoice = content;
                RecorderColor = "#240108";
                var commandRaw = speakatoRecognizer!.TextToCommand(content);

                if (commandRaw != null || content.Contains("stwórz"))
                {
                    Command command = (Command)Enum.Parse(typeof(Command), commandRaw, true);
                    SystemAnswer = commandResolver.ResolveCommnad(command, content);
                }
                else
                {
                    SystemAnswer = "Nie wykryłem żadnej komendy";
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                SystemAnswer = "Coś się popsuło, wracam na pulpit";
            }
            finally
            {
                CanListen = true;
                synthesizer.SpeakAsync(systemAnswer);

                //this.speakatoRecognizer!.ToggleListening();
            }
        }

        public ICommand CloseApplicationCommand { get; private set; }

        public void CloseApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private static byte[] GetMicrophoneInput()
        {
            byte[] result;
            using (var waveIn = new NAudio.Wave.WaveInEvent
            {
                DeviceNumber = 0,
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
                Thread.Sleep(4 * 1000);
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

        private string systemAnswer = string.Empty;
        public string SystemAnswer
        {
            get
            {
                return systemAnswer;
            }
            private set
            {
                systemAnswer = value;
                this.RaisePropertyChanged(nameof(SystemAnswer));
            }
        }

        private string recorderColor = "#240108";
        public string RecorderColor
        {
            get
            {
                return recorderColor;
            }
            private set
            {
                recorderColor = value;
                Application.Current.Dispatcher.BeginInvoke(new Action(() => RaisePropertyChanged(nameof(RecorderColor))));
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
                Application.Current.Dispatcher.BeginInvoke(new Action(() => RaisePropertyChanged(nameof(CanListen))));
                ((RelayCommand)ListenCommand).RaiseCanExecuteChanged();

            }
        }
    }

}
