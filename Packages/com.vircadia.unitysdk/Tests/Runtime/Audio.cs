//
//  Audio.cs
//  Tests/Runtime
//
//  Created by Nshan G. on 26 July 2022.
//  Copyright 2022 Vircadia contributors.
//  Copyright 2022 DigiSomni LLC.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Collections;
using System.Threading;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Audio
{
    [UnityTest]
    public IEnumerator EchoTone()
    {
        const int sampleRate = 48000;
        const int frameSize = 480;
        const int outputChannelCount = 2;
        const float toneFrequency = 148;
        const float TAU = Mathf.PI * 2;

        var output = new float[sampleRate * outputChannelCount];
        int outputSize = 0;
        var outputBuffer = new float[frameSize * outputChannelCount];
        Thread outputThread;

        var inputBuffer = new float[frameSize];
        Thread inputThread;

        var domainServer = new Vircadia.DomainServer();
        bool connected = false;

        Assert.NotNull(domainServer.Audio);
        domainServer.Audio.Enable();
        domainServer.Audio.SetEcho(true);

        domainServer.Audio.InputReady += (Vircadia.AudioInputWriter writer) =>
        {
            inputThread = new Thread(() =>
            {
                int current = 0;
                while (current < sampleRate)
                {
                    for (int i = 0; i < frameSize; ++i)
                    {
                        inputBuffer[i] = Mathf.Sin(toneFrequency * TAU * current / sampleRate);
                        ++current;
                    }
                    writer.Write(inputBuffer);
                    Thread.Sleep(10);
                }
            });
            inputThread.Start();
        };

        domainServer.Audio.OutputReady += (Vircadia.AudioOutputReader reader) =>
        {
            outputThread = new Thread(() =>
            {
                while (outputSize < output.Length)
                {
                    reader.Read(outputBuffer);
                    Array.Copy(outputBuffer, 0, output, outputSize, outputBuffer.Length);
                    outputSize += outputBuffer.Length;
                    Thread.Sleep(10);
                }
            });
            outputThread.Start();
        };

        domainServer.Audio.StartInput(new Vircadia.AudioFormat{
                sampleType = Vircadia.AudioFormat.SampleType.Float,
                sampleRate = 48000,
                channelCount = 1
        });

        domainServer.Audio.StartOutput(new Vircadia.AudioFormat{
                sampleType = Vircadia.AudioFormat.SampleType.Float,
                sampleRate = 48000,
                channelCount = 2
        });

        domainServer.Connect("localhost");

        for (int i = 0; i < 300; ++i)
        {
            var status = domainServer.Status;
            if (status == Vircadia.DomainServerStatus.Connected)
            {
                connected = true;
                domainServer.Audio.Update();
            }
            else
            {
                Assert.AreEqual(status, Vircadia.DomainServerStatus.Disconnected);
            }

            yield return new WaitForSeconds(0.01f);
        }

        if (connected)
        {
            Assert.IsTrue(output.Count(x => Mathf.Abs(x) > 0.5f) > 5000);
        }

        domainServer.Destroy();

    }
}
