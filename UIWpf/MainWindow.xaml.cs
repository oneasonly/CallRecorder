using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UIWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region fields
        WasapiCapture CaptureInstance;
        WaveOut _audioPlayer = new WaveOut();
        int count = 1;
        bool isLetClick = true;
        private readonly string recordDir = @"records";
        #endregion

        #region Private Methods
        private void OnLoad()
        {
            CallInputTextBox.Focus();
            //RecordStopButton.IsEnabled = false;
            _audioPlayer.PlaybackStopped += audioPlayer_PlaybackStopped;
        }

        private async void audioPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            await Task.Delay(350);            
            SetIconCallStart();
        }

        public async Task AppMonitoring()
        {
            bool prevCheck = false;

            while (true)
            {
                await Task.Delay(1000);

                bool isAudioPlaying = _audioPlayer.PlaybackState == PlaybackState.Playing;

                if (isAudioPlaying != prevCheck)
                {
                    
                }
                prevCheck = isAudioPlaying;
            }
        }

        private void RecordStart(string filename)
        {
            //var deviceRecorder = WasapiCapture.GetDefaultCaptureDevice();
            CaptureInstance = new WasapiCapture();
            WaveFileWriter RecordedAudioWriter = new WaveFileWriter(filename, CaptureInstance.WaveFormat);

            var hz = WasapiLoopbackCapture.GetDefaultCaptureDevice();
            var qw2 = WasapiLoopbackCapture.GetDefaultLoopbackCaptureDevice();

            CaptureInstance.DataAvailable += (s, a) =>
            {
                // Write buffer into the file of the writer instance
                RecordedAudioWriter.Write(a.Buffer, 0, a.BytesRecorded);
            };
            CaptureInstance.RecordingStopped += (s, a) =>
            {
                RecordedAudioWriter.Dispose();
                RecordedAudioWriter = null;
                CaptureInstance.Dispose();
            };

            CaptureInstance.StartRecording();
        }

        private void RecordStart()
        {
            if(!Directory.Exists(recordDir))
            {
                Directory.CreateDirectory(recordDir);
            }
            string patern = "yyyy.MM.dd hh-mm";
            string endFile = ".wav";
            string filename = $@"{recordDir}\{CallInputTextBox.Text} {DateTime.Now.ToString(patern)}";
            string fullFilename = filename + endFile;
            while (File.Exists(fullFilename))
            {
                count++;
                fullFilename = $"{filename}({count}){endFile}";
            }
            count = 1;

            RecordStart(fullFilename);
        }

        private void RecordStop()
        {
            CaptureInstance.StopRecording();
        }

        private void PlayMP3()
        {
            var reader = new Mp3FileReader("The Score - The Fear.mp3");            
            var waveOut = new WaveOut(); // or WaveOutEvent()
            waveOut.Init(reader);
            waveOut.Play();
        }

        private async void PlayAudio(string filename)
        {
            if(_audioPlayer.PlaybackState == PlaybackState.Playing)
            {
                _audioPlayer.Stop();
            }
            if (_audioPlayer.PlaybackState == PlaybackState.Paused)
            {
                _audioPlayer.Stop();
            }

            var reader = new AudioFileReader(filename);

            
            _audioPlayer.Init(reader);
            await Task.Delay(500);
            _audioPlayer.Play();

            //var reader = new AudioFileReader(filename); //The Score - The Fear.mp3 //call 2020.03.08 09-40.wav
            //_audioPlayer = new WaveOut(); // or WaveOutEvent()
        }

        private string GetFilenameByNumber()
        {
            string fullFilePath = null;
            string fileName = null;
            if (CallInputTextBox.Text == "101" || CallInputTextBox.Text == "112")
            {
                fileName = @"101.ogg";
            }
            else if (CallInputTextBox.Text == "102")
            {
                fileName = @"Fight the fear.mp3";
            }
            else if (CallInputTextBox.Text == "103")
            {

            }
            else if (CallInputTextBox.Text == "104")
            {

            }
            if (fileName == null) return null;
            fullFilePath = $@"rules\{fileName}";
            return fullFilePath;
        }

        private async Task ChangeIconCall()
        {
            await Task.Delay(320);
            if (CallIcon.Kind == MaterialDesignThemes.Wpf.PackIconKind.Call)
                SetIconCallStop();
            else
                SetIconCallStart();
        }

        private void SetIconCallStop()
        {
            CallIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CallEnd;
            CallIcon.Foreground = Brushes.OrangeRed;
        }

        private void SetIconCallStart()
        {
            CallIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Call;
            CallIcon.Foreground = Brushes.LawnGreen;
        }
        #endregion

        #region UI Methods
        public MainWindow()
        {
            InitializeComponent();
            OnLoad();            
        }

        private void RecordStartButton_Click(object sender, RoutedEventArgs e)
        {
            //RecordStartButton.IsEnabled = false;
            //RecordStopButton.IsEnabled = true;
            RecordStart();
        }

        private void RecordStopButton_Click(object sender, RoutedEventArgs e)
        {
            //RecordStopButton.IsEnabled = false;
            //RecordStartButton.IsEnabled = true;
            RecordStop();
        }

        private void PlayWavButton_Click(object sender, RoutedEventArgs e)
        {
            PlayAudio("The Score - The Fear.mp3"); //The Score - The Fear.mp3 //call 2020.03.08 09-40.wav
        }

        private void PlayMp3Button_Click(object sender, RoutedEventArgs e)
        {
            PlayMP3();
        }

        private async void CallKeyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isLetClick)
                {
                    isLetClick = false;
                    if (CallIcon.Kind == MaterialDesignThemes.Wpf.PackIconKind.CallEnd)
                    {
                        _audioPlayer.Stop();
                    }
                    else
                    {
                        string filePath = GetFilenameByNumber();
                        if (filePath != null)
                        {
                            PlayAudio(filePath);
                            RecordStart();
                            ChangeIconCall();
                        }
                    }
                    await Task.Delay(350);
                    isLetClick = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion

    }
}
