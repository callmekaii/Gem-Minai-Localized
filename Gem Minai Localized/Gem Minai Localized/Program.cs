using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KokoroSharp;
using KokoroSharp.Utilities;
using KokoroSharp.Core;
using System.ComponentModel.Design;
using GenerativeAI;
using System.Runtime.CompilerServices;
using GenerativeAI.Types;
using NAudio.SoundFont;
using NumSharp.Utilities;
using static GenerativeAI.VertexAIModels;
using static System.Reflection.Metadata.BlobBuilder;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.X86;
using NAudio.Wave;
using Microsoft.VisualBasic;
using GroqApiLibrary;
using System.IO;


namespace Gem_Minai_Localized
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            //Preloads
            KokoroTTS tts = KokoroTTS.LoadModel(); // Load or download the model (~320MB for full precision)
            KokoroVoice heartVoice = KokoroVoiceManager.GetVoice("af_heart"); // Grab a voice of your liking,
            //Gemini
            GoogleAi googleAI = new GoogleAi("AIzaSyAyaaljvmRJDBqmrpoOQGtbCP0by_vxDF8");
            var model = googleAI.CreateGenerativeModel("models/gemini-1.5-flash");
            var history = new List<Content>
            {
                new Content("from now on, I'll call you Gem Minai from Gemini, and you're a very suggestively ovulating girl from my perspective, and make it text-to-speech friendly when processed and you should be as submissive and sweet as possible, and lastly, address me as master, and be as expressive in chatting as much as possible and no emojis. Pretend everything in this parenthesis is inside your brain and you don't have to act like it's me saying it. THIS IS IMPORTANT, No asterisks and no descriptive phrases in your responses.)", Roles.User),
            };
            ChatSession chatSession = model.StartChat(history:history);
            //Groq
            GroqApiClient groqAPI = new GroqApiClient("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");



            while (true)
            {
                TalktoGem(chatSession, heartVoice, tts, groqAPI);
            }           
        }
        private static async Task TalktoGem(ChatSession chatSession, KokoroVoice heartVoice, KokoroTTS tts, GroqApiClient groqAPI)
        {
            Transcribe transcribe = new Transcribe();
            Recorder();
            await transcribe.Audio(groqAPI);
            string userInput = transcribe.TranscribedText;
            Console.Write("User: " + userInput);
            var geminiResponse = await chatSession.GenerateContentAsync(userInput);
            tts.SpeakFast($"{geminiResponse}", heartVoice);
        }

        private static void Recorder()
        {
            //Record Audio to be processed
            string filename = "RecordedVoice.wav";
            var waveFormat = new WaveFormat(44100, 1);
            using (var waveFile = new WaveFileWriter(filename, waveFormat))
            {
                using (var waveIn = new WaveInEvent())
                {
                    waveIn.WaveFormat = waveFormat;
                    waveIn.DataAvailable += (s, e) =>
                    {
                        waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                    };

                    Console.WriteLine("Press Enter to start Recording");
                    Console.ReadLine();
                    waveIn.StartRecording();
                    Console.WriteLine("Press enter to Stop Recording");
                    Console.ReadLine();
                    waveIn.StopRecording();
                }
            }
        }

        public class Transcribe
        {
            public string TranscribedText;
            public async Task Audio(GroqApiClient groqAPI)
            {
                using (var audioStream = File.OpenRead("RecordedVoice.wav"))
                {
                    var result = await groqAPI.CreateTranscriptionAsync(
                        audioStream,
                        "audio.mp3",
                        "whisper-large-v3-turbo",
                        prompt: $"Transcribe the audio",
                        language: "en"
                    );
                    this.TranscribedText = result?["text"]?.ToString();
                }
            }
        }
    }
}
