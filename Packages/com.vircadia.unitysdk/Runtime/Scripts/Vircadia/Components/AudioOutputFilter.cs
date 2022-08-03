//
//  AudioOutputFilter.cs
//  Runtime/Scripts/Vircadia/Components
//
//  Created by Nshan G. on 24 July 2022.
//  Copyright 2022 Vircadia contributors.
//  Copyright 2022 DigiSomni LLC.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Vircadia
{

    /// <summary>
    /// Audio output filter component. This will overwrite the audio data with
    /// data received from the mixer. Meant to be used as the first filter in
    /// the chain, without an AudioClip.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioOutputFilter : MonoBehaviour
    {
        /// <summary>
        /// <see cref="Vircadia.AudioOutputReader"> AudioOutputReader </see>
        /// object to use to receive audio data, provided by <see
        /// cref="Vircadia.AudioClient.OutputReady"> AudioClient.OutputReady
        /// </see> event.
        /// </summary>
        public AudioOutputReader reader;

        void OnAudioFilterRead(float[] samples, int channels)
        {
            reader.Read(samples);
        }
    }

}
