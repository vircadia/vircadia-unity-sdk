//
//  AudioCallScript.cs
//  Samples/AudioCall/Assets
//
//  Connect to a domain as a spooky disembodied voice~ i.e. basic audio input and output.
//
//  Created by Nshan G. on 24 July 2022.
//  Copyright 2022 Vircadia contributors.
//  Copyright 2022 DigiSomni LLC.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using UnityEngine;


public class AudioCallScript : MonoBehaviour
{
    void Start()
    {
        _domainServer = new Vircadia.DomainServer(
            appName: Application.productName,
            appOrganization: Application.companyName,
            appDomain: "",
            appVersion: Application.version);
        _connected = false;

        if (!string.IsNullOrEmpty(location))
        {
            _domainServer.Connect(location);

            _domainServer.Audio.Enable();
            _domainServer.Audio.AudioInputReady += StartInput;
            _domainServer.Audio.AudioOutputReady += StartOutput;

            _domainServer.Audio.SetBounds(new Bounds(Vector3.one * 0.5f, Vector3.one));
            _domainServer.Audio.SetTransform(vantagePoint);
            _domainServer.Audio.SetEcho(echo);
            _domainServer.Audio.SetInputMuted(muteInput);

            _domainServer.Audio.StartInput(new Vircadia.AudioFormat{
                sampleType = Vircadia.AudioFormat.SampleType.Float,
                sampleRate = AudioSettings.outputSampleRate,
                channelCount = isStereoInput ? 2 : 1
            });

            _domainServer.Audio.StartOutput(new Vircadia.AudioFormat{
                sampleType = Vircadia.AudioFormat.SampleType.Float,
                sampleRate = AudioSettings.outputSampleRate,
                channelCount = AudioSettings.speakerMode == AudioSpeakerMode.Mono ? 1 :
                    AudioSettings.speakerMode == AudioSpeakerMode.Stereo ? 2 :
                    AudioSettings.speakerMode == AudioSpeakerMode.Quad ? 4 :
                    AudioSettings.speakerMode == AudioSpeakerMode.Surround ? 5 :
                    AudioSettings.speakerMode == AudioSpeakerMode.Mode5point1 ? 6 :
                    AudioSettings.speakerMode == AudioSpeakerMode.Mode7point1 ? 8 :
                    AudioSettings.speakerMode == AudioSpeakerMode.Prologic ? 2 :
                    2
            });
        }
    }

    void StartInput(Vircadia.AudioInputWriter writer)
    {
        var audioInput = new GameObject("AudioInput");
        AudioSource audioSource = audioInput.AddComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
        var inputFilter = audioInput.AddComponent<Vircadia.AudioInputFilter>();
        inputFilter.writer = writer;
        inputFilter.expectedChannelCount = isStereoInput ? 2 : 1;
        audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { } // Weird, but without this the audio source gets ahead of the clip.
        // TODO: Try using a coroutine instead of a busy wait, may affect latency.
        // NOTE: The audio source still occasionally gets ahead of the
        // microphone, which result in a repeat of the last part of recording
        // and hight latency after that (probably has to do with lenghtSec in
        // Microphone.Start). Might be possible to detect and mitigate by
        // restarting the microphone and the audioSource.
        audioSource.Play();
    }

    void StartOutput(Vircadia.AudioOutputReader reader)
    {
        var audioOutput = new GameObject("AudioOutput");
        AudioSource audioSource = audioOutput.AddComponent<AudioSource>();
        var outputFilter = audioOutput.AddComponent<Vircadia.AudioOutputFilter>();
        outputFilter.reader = reader;
        audioSource.Play();
    }

    void Update()
    {
        if (!_connected)
        {
            Debug.Log("[vircadia-unity-example] Connecting");
            if (_domainServer.Status == Vircadia.DomainServerStatus.Connected)
            {
                Debug.Log("[vircadia-unity-example] Successfully connected to " + location);
                _connected = true;
            }
        }
        else
        {
            _domainServer.Audio.SetTransform(vantagePoint);
            _domainServer.Audio.SetEcho(echo);
            _domainServer.Audio.SetInputMuted(muteInput);
            _domainServer.Audio.Update();

        }
    }

    void OnDestroy()
    {
        _domainServer.Destroy();
    }

    public string location = "localhost";
    public bool isStereoInput = false;
    public Vircadia.Transform vantagePoint = new Vircadia.Transform {
        translation = Vector3.zero,
        rotation = Quaternion.identity,
        scale = 1
    };
    public bool echo = false;
    public bool muteInput = false;

    private Vircadia.DomainServer _domainServer;
    private bool _connected;
}
