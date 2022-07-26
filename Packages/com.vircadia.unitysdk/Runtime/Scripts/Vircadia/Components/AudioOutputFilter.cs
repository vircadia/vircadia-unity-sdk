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

    public class AudioOutputFilter : MonoBehaviour
    {

        public AudioOutputReader reader;

        void OnAudioFilterRead(float[] samples, int channels)
        {
            reader.Read(samples);
        }

    }

}
