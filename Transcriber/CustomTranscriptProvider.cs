using System;
using System.Net;
using System.Speech.Recognition;
using System.Threading;
using System.Threading.Tasks;

namespace Transcriber
{
    public class CustomTranscriptProvider
    {
        public string Text { get; set; }
        public bool  IsCompleted { get; set; }

     
       

        public Task<string> GetTranscriptFromWavFile(string wavFileLocation)
        {
            try
            {
                IsCompleted = false;
                Text = string.Empty;
                var speechRecogEngine = new SpeechRecognitionEngine();
                //speechRecogEngine.SetInputToDefaultAudioDevice();

                //speechRecogEngine.SetInputToWaveFile("D:\\Projects_Mine\\VoiceRecognition\\VoiceRecognition\\RE4b378437406423c181d4d111b3dfdde5.wav");
                //speechRecogEngine.SetInputToWaveFile(wavFileLocation);

                var audioFile = Guid.NewGuid().ToString();
                audioFile = $"D:\\Projects\\ivr\\Transcriber\\{audioFile}.wav";
                using (var client = new WebClient())
                {
                    client.DownloadFile(wavFileLocation, audioFile);
                }

                speechRecogEngine.LoadGrammar(new DictationGrammar());
                speechRecogEngine.SetInputToWaveFile(audioFile);

                //speechRecogEngine.SetInputToDefaultAudioDevice();
                //speechRecogEngine.RecognizeAsync();
                //speechRecogEngine.RecognizeCompleted += SpeechRecogEngine_RecognizeCompleted;
                
                var r = speechRecogEngine.Recognize();
                speechRecogEngine.RecognizeCompleted += SpeechRecogEngine_RecognizeCompleted;
              
                //var task = Task.Run(() =>
                //{
                //    speechRecogEngine.RecognizeAsync();
                //    speechRecogEngine.RecognizeCompleted += SpeechRecogEngine_RecognizeCompleted;
                //});
                //task.Wait();
                //while (!IsCompleted)
                //{
                //    Thread.Sleep(333);
                //}

                return Task.FromResult(r.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SpeechRecogEngine_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            try
            {
                Text = e.Result.Text;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            IsCompleted = true;
        }
    }
}
