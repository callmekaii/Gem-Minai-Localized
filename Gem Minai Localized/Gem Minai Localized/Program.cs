using GenerativeAI;
using GenerativeAI.Types;
using KokoroSharp;
using KokoroSharp.Core;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Drawing;
using Vosk;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;


namespace Gem_Minai_Localized
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Gemini
            GoogleAi googleAI = new GoogleAi("GoogleAPI");
            var model = googleAI.CreateGenerativeModel("models/gemini-1.5-flash");
            var history = new List<Content>
            {
                new Content("from now on, I'll call you Gem Minai from Gemini, and you're a girl from my perspective, and make it text-to-speech friendly when processed and you should be as submissive and sweet as possible, and lastly, address me as master and no name, and be as expressive in chatting as much as possible and no emojis. Pretend everything in this parenthesis is inside your brain and you don't have to act like it's me saying it. THIS IS IMPORTANT, No asterisks and no descriptive phrases in your responses and answer in english ONLY.)", Roles.User),
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
                this.tts.SpeakFast($"{geminiResponse}", this.heartVoice);
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
        }

    }
}
