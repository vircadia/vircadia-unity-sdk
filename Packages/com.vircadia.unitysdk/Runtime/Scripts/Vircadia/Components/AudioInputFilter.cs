//
//  AudioInputFilter.cs
//  Runtime/Scripts/Vircadia/Components
//
//  Created by Nshan G. on 24 July 2022.
//  Copyright 2022 Vircadia contributors.
//  Copyright 2022 DigiSomni LLC.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using UnityEngine;

namespace Vircadia
{

    public class AudioInputFilter : MonoBehaviour
    {

        public AudioInputWriter writer;
        public int expectedChannelCount;

        void OnAudioFilterRead(float[] samples, int channelCount)
        {
            if (expectedChannelCount == channelCount)
            {
                writer.Write(samples);
            }
            // Unity automatically converts everything to output
            // format, so channelCount passed here has little to do
            // with microphone input and we have to do these
            // conversions to go back to what we expect.
            else if (expectedChannelCount == 1 && channelCount > 1)
            {
                for (int i = 0, o = 0; i < samples.Length; i += channelCount, ++o)
                {
                    samples[o] = samples[i];
                    for (int j = 1; j < channelCount; ++j)
                    {
                        samples[o] += samples[i+j];
                    }
                    samples[o] /= channelCount;
                    samples[o] = Mathf.Clamp(samples[o], -1, 1);
                }

                writer.Write(samples, samples.Length / channelCount);
            }
            else if (expectedChannelCount == 2 && channelCount == 1)
            {
                var stereoSamples = new float[samples.Length * 2];
                for (int i = 0 ; i < samples.Length; ++i)
                {
                    stereoSamples[i*2] = samples[i];
                    stereoSamples[i*2 + 1] = samples[i];
                }
                writer.Write(stereoSamples);
            }
            else
            {
                // TODO: unsupported channel configuration, signal an
                // error somehow, this function runs on a different
                // thread
            }

            for (int i = 0; i < samples.Length; ++i)
            {
                samples[i] = 0;
            }
        }

    }

}
