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
        #endregion

        #region Private Methods
        private void OnLoad()
        {
            RecordStopButton.IsEnabled = false;
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
            string patern = "yyyy.MM.dd hh-mm";
            string endFile = ".wav";
            string filename = $@"records\mic {DateTime.Now.ToString(patern)}";
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

        private void PlayAudio()
        {
            PlayAudio("The Score - The Fear.mp3"); //The Score - The Fear.mp3 //call 2020.03.08 09-40.wav
        }
        private void PlayAudio(string filename)
        {
            if(_audioPlayer.PlaybackState == PlaybackState.Playing)
            {
                _audioPlayer.Pause();
            }
            else if (_audioPlayer.PlaybackState == PlaybackState.Paused)
            {
                _audioPlayer.Resume();
            }
            else
            {
                var reader = new AudioFileReader(filename); //The Score - The Fear.mp3 //call 2020.03.08 09-40.wav
                //_audioPlayer = new WaveOut(); // or WaveOutEvent()
                _audioPlayer.Init(reader);
                _audioPlayer.Play();
            }
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
            RecordStartButton.IsEnabled = false;
            RecordStopButton.IsEnabled = true;
            RecordStart();
        }

        private void RecordStopButton_Click(object sender, RoutedEventArgs e)
        {
            RecordStopButton.IsEnabled = false;
            RecordStartButton.IsEnabled = true;
            RecordStop();
        }
        #endregion

        private void PlayWavButton_Click(object sender, RoutedEventArgs e)
        {
            PlayAudio();
        }

        private void PlayMp3Button_Click(object sender, RoutedEventArgs e)
        {
            PlayMP3();
        }
    }
}
