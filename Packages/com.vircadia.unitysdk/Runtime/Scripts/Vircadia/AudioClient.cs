//
//  AudioClient.cs
//  Runtime/Scripts/Vircadia
//
//  Created by Nshan G. on 23 July 2022.
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

    public struct AudioFormat
    {
        /// <summary>
        /// Supported audio sample types.
        /// </summary>
        public enum SampleType : byte
        {

            /// <summary>
            /// Signed 16 but little endian integer.
            /// </summary>
            SignedInt16,

            /// <summary>
            /// IEEE 754 single precision floating point number.
            /// </summary>
            Float
        }

        internal VircadiaNative.audio_format toNative()
        {
            return new VircadiaNative.audio_format {
                sample_type = (byte) sampleType,
                sample_rate = sampleRate,
                channel_count = channelCount
            };
        }

        public SampleType sampleType;
        public int sampleRate;
        public int channelCount;
    }

    public class AudioInputWriter
    {
        internal AudioInputWriter(IntPtr context)
        {
            _context = context;
        }

        public void Write(float[] interleavedSamples, int length = -1)
        {
            if (length == -1)
            {
                length = interleavedSamples.Length;
            }

            unsafe
            {
                fixed (float* ptr = interleavedSamples)
                {
                    VircadiaNative.Audio.vircadia_set_audio_input_data(_context, (IntPtr)ptr, length * sizeof(float));
                }
            }
        }

        IntPtr _context;
    }

    public class AudioOutputReader
    {
        internal AudioOutputReader(IntPtr context)
        {
            _context = context;
        }

        public void Read(float[] interleavedSamples)
        {
            unsafe
            {
                fixed (float* ptr = interleavedSamples)
                {
                    VircadiaNative.Audio.vircadia_get_audio_output_data(_context, (IntPtr)ptr, interleavedSamples.Length * sizeof(float));
                }
            }
        }

        private IntPtr _context;
    }

    public delegate void AudioInputReadyHandler(AudioInputWriter input);

    public delegate void AudioOutputReadyHandler(AudioOutputReader output);

    public class AudioClient
    {
        public event AudioInputReadyHandler AudioInputReady = delegate {};
        public event AudioOutputReadyHandler AudioOutputReady = delegate {};

        public void Enable()
        {
            VircadiaNative.Audio.vircadia_enable_audio(_context);

            int audioBufferSize = 0;
            int _ = 0;
            AudioSettings.GetDSPBufferSize(out audioBufferSize, out _);
            // TODO: compute output frame count based on buffer size
            VircadiaNative.Audio.vircadia_set_audio_output_buffer_frames(_context, 2);
        }

        public void Update()
        {
            IntPtr inputPtr = VircadiaNative.Audio.vircadia_get_audio_input_context(_context);
            Input = (inputPtr != IntPtr.Zero) ? new AudioInputWriter(inputPtr) : null;
            if (_inputRequested && Input != null)
            {
                AudioInputReady(Input);
                _inputRequested = false;
            }

            IntPtr outputPtr = VircadiaNative.Audio.vircadia_get_audio_output_context(_context);
            Output = (outputPtr != IntPtr.Zero) ? new AudioOutputReader(outputPtr) : null;
            if (_outputRequested && Output != null)
            {
                AudioOutputReady(Output);
                _outputRequested = false;
            }
        }

        public void StartInput(AudioFormat format)
        {
            VircadiaNative.Audio.vircadia_set_audio_input_format(_context, format.toNative());
            _inputRequested = true;
        }

        public void StartOutput(AudioFormat format)
        {
            VircadiaNative.Audio.vircadia_set_audio_output_format(_context, format.toNative());
            _outputRequested = true;
        }

        public void SetEcho(bool enabled)
        {
            VircadiaNative.Audio.vircadia_set_audio_input_echo(_context, (byte) (enabled ? 1 : 0));
        }

        public void SetInputMuted(bool enabled)
        {
            VircadiaNative.Audio.vircadia_set_audio_input_muted(_context, (byte) (enabled ? 1 : 0));
        }

        public bool GetInputMutedByMixer()
        {
            return VircadiaNative.Audio.vircadia_get_audio_input_muted_by_mixer(_context) == 1;
        }

        public void SetTransform(Transform transform)
        {
            VircadiaNative.Audio.vircadia_set_audio_vantage(_context,
                new VircadiaNative.vantage {
                    position = DataConversion.NativeVectorFrom(transform.translation),
                    rotation = DataConversion.NativeQuaternionFrom(transform.rotation)
                });
        }

        public void SetBounds(Bounds bounds)
        {
            VircadiaNative.Audio.vircadia_set_audio_bounds(_context,
                DataConversion.NativeBoundsFrom(bounds));
        }

        public AudioInputWriter Input { get; private set; }
        public AudioOutputReader Output { get; private set; }

        internal AudioClient(DomainServer domainServer)
        {
            _context = domainServer.ContextId;
            _inputRequested = false;
            _outputRequested = false;
        }

        private int _context;
        bool _inputRequested;
        bool _outputRequested;
    }

}
