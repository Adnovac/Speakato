# Speakato
Speakato is a library that implements command recognition and do not require any knowledge about the Machine Learning. It's strongly related to [SpeakatoTrainer](https://github.com/Adnovac/SpeakatoTrainer).

This project uses a modified [SpacyDotNet](https://github.com/AMArostegui/SpacyDotNet) and ```ML.NET```.

## Requirements
Account in Google Cloud/Microsoft Cognitive Services is required to use this library.

## Setup
- Create a Virtual environment with packages installed from the [requirements.txt](https://github.com/Adnovac/SpeakatoTrainer/blob/main/requirements.txt)
- Train a model using [SpeakatoTrainer](https://github.com/Adnovac/SpeakatoTrainer)

A setup script will be provided in the future.

## Initialization
In order to initialize the ```SpeakatoRecognizer``` you need to provide ```GoogleCloudConfiguration``` or ```CognitiveServiceConfiguration``` with necessary credentials. Due to the fact, that model loading can take a while, ```loadModel``` parameter specifies whether to load it in the constructor. If ```loadModel``` is set to ```false``` it's required to call ```LoadModel``` method before any other.

Example configuration:
```c#
    var config = new CognitiveServiceConfiguration
    {
        Key = {your_key},
        Url = {your_url},
        ModelPath = {speakato_model_location},
        PythonEnvironmentPath = {path_to_the_virtual_env}
    };

    var speakatoRecognizer = new SpeakatoRecognizer(new HttpClient(), config, loadModel = true);
```

> :warning: ***In order to use Google Cloud*** GOOGLE_APPLICATION_CREDENTIALS environment variable with a path to the key.json file has to be set.

## Available commands
### ```void LoadModel()```
Loads a model. Call it before first use of any other method if loadModel was set to false in the constructor.

### ```Task<string> SpeechToCommand(Stream stream)```
Predicts command with a model trained in SpeakatoTrainer. Returns null if command isn't recognized. 

```stream``` - Stream of a recording containing a sample with speech to be recognized

### ```Task<string> SpeechToText(Stream stream)```
Returns recognized speech from the given stream if possible. Returns null if speech isn't recognized. 

```stream``` - Stream of a recording containing a sample with speech to be recognized
### ```string TextToCommand(string sentence)```
Predicts command with a model trained in SpeakatoTrainer. Returns null if command isn't recognized.
```sentence``` - Sentence from which a command should be recognized

## Examples
An example use of Speakato is provided in [examples/SpeakatoVoiceAssistant](https://github.com/Adnovac/Speakato/tree/main/examples/SpeakatoVoiceAssistant) and [examples/Speakato.TestConsole](https://github.com/Adnovac/Speakato/tree/main/examples/Speakato.TestConsole). 
SpeakatoVoiceAssistant uses the model trained the [polish_commands_dataset](https://github.com/Adnovac/SpeakatoTrainer/tree/main/examples/polish_commands_dataset)
