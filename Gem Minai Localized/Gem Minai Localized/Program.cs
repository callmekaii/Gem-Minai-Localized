using GenerativeAI;
using GenerativeAI.Types;
using KokoroSharp;
using KokoroSharp.Core;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using Vosk;


namespace Gem_Minai_Localized
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Gemini
            GoogleAi googleAI = new GoogleAi("Gemini API");
            var model = googleAI.CreateGenerativeModel("models/gemini-1.5-flash");
            var history = new List<Content>
            {
                new Content("From now on, your name is Gem Minai, a helpful, friendly, and concise AI assistant. Speak clearly, answer in a calm tone, and always use English. Keep your responses text-to-speech friendly.", Roles.User),
            };
            ChatSession chatSession = model.StartChat(history:history);

            //Vosk
            Vosk.Model VoskModel = new Vosk.Model("C:\\Users\\kaise\\source\\repos\\Gem Minai Localized\\Gem Minai Localized\\vosk-model-small-en-us-0.15\\");
            var voskRecognizer = new VoskRecognizer(VoskModel, 16000);
            Processor transcribe = new Processor();

            transcribe.Core(voskRecognizer, chatSession);
             
        }

        public class Processor
        {
            //Preloads
            KokoroTTS tts = KokoroTTS.LoadModel(); // Load or download the model (~320MB for full precision)
            KokoroVoice heartVoice = KokoroVoiceManager.GetVoice("af_heart"); // Voice Load
            public string TranscribedText;
            public GenerateContentResponse geminiResponse;
            List<String> triggerWords = ["jim", "jen", "gem", "gym"];
            public void Core(VoskRecognizer VoskRecognizer, ChatSession chatSession)
            {
                //Record Audio to be processed
                var waveFormat = new WaveFormat(19000, 1);
                using (var waveIn = new WaveInEvent())
                {
                    waveIn.WaveFormat = waveFormat;
                    waveIn.BufferMilliseconds = 2000; // Set the buffer size to 2 seconds
                    waveIn.DataAvailable += (s, e) =>
                    {
                        if (VoskRecognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
                        {
                            //Parse the result json string
                            JObject jsonObject = JObject.Parse(VoskRecognizer.Result());
                            this.TranscribedText = (string)jsonObject["text"];
                            string userInput = this.TranscribedText;

                            if (!userInput.Equals(""))
                            {
                                Console.Write("User: " + userInput + "\n");

                                //Differentiates screenshotting commands and trigger words
                                if (userInput.Contains("screenshot"))
                                {
                                    //Saves the screenshot and Gem processes the image
                                    SaveScreenShot();
                                    ProcessScreenshot(chatSession);
                                }

                                else
                                {
                                    CheckTriggerWords(userInput, chatSession);
                                }

                            }
                        }
                    };
                    waveIn.StartRecording();
                    Console.WriteLine("Press enter to Stop Recording. Say anything...");
                    Console.ReadLine();
                    waveIn.StopRecording();
                }
            }

            public async void Respond(string userInput, ChatSession chatSession)
            {
                geminiResponse = await chatSession.GenerateContentAsync(userInput);
                Console.WriteLine("Gem Minai: " + geminiResponse);
                this.tts.SpeakFast($"{geminiResponse}", heartVoice);
            }

            public static void SaveScreenShot()
            {
                Console.WriteLine("Screenshot detected");
                Console.WriteLine("Capturing Image...");
                var image = ScreenCapture.CaptureDesktop();
                image.Save(@"snippetsource.jpg", ImageFormat.Jpeg);
                Console.WriteLine("Image Saved!");
            }

            public async void ProcessScreenshot(ChatSession chatSession)
            {
                var request = new GenerateContentRequest();
                request.AddText("Process the image and give me a 10-word-summary of it. Answer it for me too if it's a question");
                request.AddInlineFile("snippetsource.jpg");
                geminiResponse = await chatSession.GenerateContentAsync(request);
                Console.WriteLine("Gem Minai: " + geminiResponse);
                this.tts.SpeakFast($"{geminiResponse}", this.heartVoice);
            }

            public void CheckTriggerWords(string userInput, ChatSession chatSession)
            {
                foreach (string triggerWord in triggerWords)
                {
                    if (userInput.Contains(triggerWord))
                    {
                        Console.WriteLine("Trigger word detected: " + triggerWord);
                        Respond(userInput, chatSession);
                    }
                }
            }
        }

    }
}
