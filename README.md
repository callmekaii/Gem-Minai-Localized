[![SVG Banners](https://svg-banners.vercel.app/api?type=luminance&text1=Gem-Minai&width=800&height=200)](https://github.com/Akshay090/svg-banners)

## Gem-Minai-Localized
Gem Minai Localized is a .NET console application that provides a live voice interface to a custom Gemini AI persona using voice input and text-to-speech output. Interact with your personal AI assistant, "Gem Minai," by speaking commands after using a trigger word.

## Features

* **Voice Interaction:** Speak to the AI using your microphone.
* **Real-time Transcription:** Uses Vosk for accurate, offline speech-to-text transcription.
* **Gemini Integration:** Leverages Google's Gemini model for natural language understanding and response generation.
* **Custom Persona:** Configured with a specific persona ("Gem Minai") for a personalized interaction experience.
* **Voice Output:** Utilizes KokoroTTS for generating natural-sounding voice responses.
* **Trigger Word Activation:** Responses are processed only when a predefined trigger word is detected in your spoken input.


## Prerequisites

Before you can run this application, you will need:

* **.NET SDK:** Make sure you have the appropriate .NET SDK installed (check the project file for the target framework).
* **Google AI API Key:** Obtain an API key from the Google AI Studio (https://aistudio.google.com/).
* **Vosk Speech Recognition Model:** Download a compatible English language model from the Vosk website (e.g., `vosk-model-en-us-0.22`). You can find models here: [https://alphacephei.com/vosk/models](https://alphacephei.com/vosk/models)
* **KokoroTTS Model:** The application will attempt to load or download the necessary KokoroTTS model upon startup.

## Setup

1.  **Clone the repository:**
    ```bash
    gh repo clone callmekaii/Gem-Minai-Localized
    cd Gem-Minai-Localized
    ```
2.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```
3.  **Configure Google AI API Key:**
    Open the `Program.cs` file. Locate the line:
    ```csharp
    GoogleAi googleAI = new GoogleAi("Insert GoogleAPI");
    ```
    Replace `"Insert GoogleAPI"` with your actual Google AI API key.
4.  **Configure Vosk Model Path:**
    Download your chosen Vosk English model. Extract the downloaded archive.
    Open the `Program.cs` file. Locate the line:
    ```csharp
    Vosk.Model VoskModel = new Vosk.Model("Gem Minai Localized\\Gem Minai Localized\\vosk-model-en-us-0.22\\");
    ```
    Replace `"Gem Minai Localized\\Gem Minai Localized\\vosk-model-en-us-0.22\\"` with the absolute path to your extracted Vosk model directory.
5.  **Configure Trigger Words (Optional):**
    In the `Processor` class within `Program.cs`, you can modify the `triggerWords` list if you want to change the words that activate the AI response.
    ```csharp
    List<String> triggerWords = ["jim", "jen", "gem", "gym"];
    ```

## Usage

1.  **Run the application:**
    ```bash
    dotnet run
    ```
2.  The application will start and you will see a message indicating it is recording and waiting for input.
3.  **Speak to the AI:** Say something including one of the configured trigger words (by default: "jim", "jen", "gem", "gym"). For example, you could say, "Gem, what is the weather like today?"
4.  When a trigger word is detected, the application will send your transcribed speech to Gemini.
5.  Gemini's response will be printed to the console and spoken aloud using text-to-speech.
6.  The application will continue listening until you press Enter in the console window.

## Configuration Details

* **API Key:** Set in `Program.cs` as described in the Setup section.
* **Vosk Model Path:** Set in `Program.cs` as described in the Setup section.
* **AI Persona:** The initial persona for "Gem Minai" is defined in the `history` list when the `ChatSession` is started in the `Main` method. You can modify this initial message to change the AI's behavior.
* **Trigger Words:** Configured in the `triggerWords` list within the `Processor` class in `Program.cs`.

## Dependencies

This project uses the following libraries:

* `GenerativeAI`: For interacting with the Google Gemini API.
* `KokoroSharp`: For Text-to-Speech functionality.
* `NAudio`: For audio recording from the microphone.
* `Newtonsoft.Json`: For parsing the JSON output from Vosk.
* `Vosk`: For Speech-to-Text transcription.
