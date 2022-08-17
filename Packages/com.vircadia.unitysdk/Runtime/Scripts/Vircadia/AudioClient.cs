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

    /// <summary>
    /// Represents format of audio data. Samples of multiple channels are
    /// always interleaved.
    /// </summary>
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

        /// <summary>
        /// The type of a single audio sample.
        /// </summary>
        public SampleType sampleType;

        /// <summary>
        /// The number of samples per channel that correspond to 1 second of
        /// audio (usually between 8000 to 96000).
        /// </summary>
        public int sampleRate;

        /// <summary>
        /// The number of audio channels (usually between 1 to 8).
        /// </summary>
        public int channelCount;
    }

    /// <summary>
    /// A handle for sending audio data to the mixer, possible on a separate thread.
    /// </summary>
    public class AudioInputWriter
    {
        internal AudioInputWriter(IntPtr context, int channelCount)
        {
            _context = context;
            this.channelCount = channelCount;
        }

        /// <summary>
        /// Writes audio data to an internal buffer to be sent to the mixer
        /// periodically. This function can be called from a separate audio
        /// precessing thread.
        /// <param name="interleavedSamples">
        /// Audio data buffer in the format specified by the last call to <see
        /// cref="Vircadia.AudioClient.StartInput"> StartInput </see>.
        /// </param>
        /// <param name="length">
        /// Optional count of audio samples to be written. By default the
        /// entire buffer is written.
        /// </param>
        /// </summary>
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

        internal int channelCount;
        private IntPtr _context;
    }

    /// <summary>
    /// A handle for receiving audio data from the mixer, possible on a separate thread.
    /// </summary>
    public class AudioOutputReader
    {
        internal AudioOutputReader(IntPtr context)
        {
            _context = context;
        }

        /// <summary>
        /// Reads audio data from an internal buffer that is periodically
        /// updated with data received from the mixer. This function can be
        /// called from a separate audio precessing thread.
        /// <param name="interleavedSamples">
        /// Audio data buffer to read into. The data will be converted to the
        /// format specified by the last call to <see
        /// cref="Vircadia.AudioClient.StartOutput"> StartOutput </see>.
        /// </param>
        /// </summary>
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


    /// <summary>
    /// Event handler type for starting audio input.
    /// </summary>
    /// <param name="input">The new handle to use for writing.</param>
    public delegate void AudioInputReadyHandler(AudioInputWriter input);

    /// <summary>
    /// Event handler type for starting audio output.
    /// </summary>
    /// <param name="output">The new handle to use for reading.</param>
    public delegate void AudioOutputReadyHandler(AudioOutputReader output);

    /// <summary>
    /// An interface for sending and receiving audio data, available as <see
    /// cref="Vircadia.DomainServer.Audio"> DomainServer.Audio </see>. The <see
    /// cref="Vircadia.AudioClient.Enable"> Enable </see> method must be called
    /// first, afterwards <see cref="Vircadia.AudioClient.Update"> Update
    /// </see> method must be called periodically.
    /// </summary>
    public class AudioClient
    {
        /// <summary>
        /// Fires when the client is ready to start sending audio data to the mixer.
        /// </summary>
        public event AudioInputReadyHandler InputReady = delegate {};

        /// <summary>
        /// Fires when the client is ready to start receiving audio data from the mixer.
        /// </summary>
        public event AudioOutputReadyHandler OutputReady = delegate {};

        /// <summary>
        /// Enables handling of audio. This must be called first.
        /// </summary>
        public void Enable()
        {
            VircadiaNative.Audio.vircadia_enable_audio(_context);

            int audioBufferSize = 0;
            int _ = 0;
            AudioSettings.GetDSPBufferSize(out audioBufferSize, out _);
            // TODO: compute output frame count based on buffer size
            VircadiaNative.Audio.vircadia_set_audio_output_buffer_frames(_context, 2);
        }

        /// <summary>
        /// Updates properties and delivers events, must be called periodically.
        /// </summary>
        public void Update()
        {
            IntPtr inputPtr = VircadiaNative.Audio.vircadia_get_audio_input_context(_context);
            Input = (inputPtr != IntPtr.Zero) ? new AudioInputWriter(inputPtr, _inputFormat.channelCount) : null;
            if (_inputRequested && Input != null)
            {
                InputReady(Input);
                _inputRequested = false;
            }

            IntPtr outputPtr = VircadiaNative.Audio.vircadia_get_audio_output_context(_context);
            Output = (outputPtr != IntPtr.Zero) ? new AudioOutputReader(outputPtr) : null;
            if (_outputRequested && Output != null)
            {
                OutputReady(Output);
                _outputRequested = false;
            }
        }

        /// <summary>
        /// Starts the audio input. This is an asynchronous process, that
        /// completes with <see cref="Vircadia.AudioClient.InputReady">
        /// InputReady </see> event, that provides a handle for writing audio
        /// data in specified format.
        /// <param name="format">
        /// The format of the input audio data. The channel count must be 1 or 2.
        /// <param>
        /// </summary>
        public void StartInput(AudioFormat format)
        {
            VircadiaNative.Audio.vircadia_set_audio_input_format(_context, format.toNative());
            _inputRequested = true;
            _inputFormat = format;
        }

        /// <summary>
        /// Starts the audio output. This is an asynchronous process, that
        /// completes with <see cref="Vircadia.AudioClient.OutputReady">
        /// OutputReady </see> event, that provides a handle for reading audio
        /// data in specified format.
        /// <param name="format"> The format of the output audio data. <param>
        /// </summary>
        public void StartOutput(AudioFormat format)
        {
            VircadiaNative.Audio.vircadia_set_audio_output_format(_context, format.toNative());
            _outputRequested = true;
        }

        /// <summary>
        /// Determines whether the mixer should send the audio input back to
        /// this client. By default input is not sent back.
        /// <param name="enabled"> true - send input back, false - do not send input back. <param>
        /// </summary>
        public void SetEcho(bool enabled)
        {
            VircadiaNative.Audio.vircadia_set_audio_input_echo(_context, (byte) (enabled ? 1 : 0));
        }

        /// <summary>
        /// Convenience method to mute audio input client side.
        /// <param name="enabled"> true - input muted, false - input not muted. <param>
        /// </summary>
        public void SetInputMuted(bool enabled)
        {
            VircadiaNative.Audio.vircadia_set_audio_input_muted(_context, (byte) (enabled ? 1 : 0));
        }

        /// <summary>
        /// Check if this client has been muted by the mixer.
        /// </summary>
        /// <returns> true - has been muted, false - has not been muted</returns>
        public bool GetInputMutedByMixer()
        {
            return VircadiaNative.Audio.vircadia_get_audio_input_muted_by_mixer(_context) == 1;
        }

        /// <summary>
        /// Sets the position and orientation of the audio client.
        /// </summary>
        /// <param name="transform">
        /// Linear transformation that determines the position and orientation.
        /// The scale is ignored.
        /// </param>
        public void SetTransform(Transform transform)
        {
            VircadiaNative.Audio.vircadia_set_audio_vantage(_context,
                new VircadiaNative.vantage {
                    position = DataConversion.NativeVectorFrom(transform.translation),
                    rotation = DataConversion.NativeQuaternionFrom(transform.rotation)
                });
        }

        /// <summary>
        /// Sets the local bounding box the audio client. Must be set to a non
        /// zero size to receive audio output from the mixer.
        /// </summary>
        /// <param name="bounds"> The bounding box to set. </param>
        public void SetBounds(Bounds bounds)
        {
            VircadiaNative.Audio.vircadia_set_audio_bounds(_context,
                DataConversion.NativeBoundsFrom(bounds));
        }

        /// <summary>
        /// A handle for sending audio data to the mixer. Initially set to
        /// null. After a call to <see cref="Vircadia.AudioClient.StartInput">
        /// StartInput </see> will be set to valid handle once it's ready.
        /// Subsequent calls to StartInput will invalidate the current handle
        /// and set this to property to null, until a new handle is ready. <see
        /// cref="Vircadia.AudioClient.Update"> Update </see> method must be
        /// called to update this property. Alternatively the <see
        /// cref="Vircadia.AudioClient.InputReady"> InputReady </see> event can
        /// be used, retrieve the same handle.
        /// </summary>
        public AudioInputWriter Input { get; private set; }

        /// <summary>
        /// A handle for receiving audio data from the mixer. Initially set to
        /// null. After a call to <see cref="Vircadia.AudioClient.StartOutput">
        /// StartOutput </see> will be set to valid handle once it's ready.
        /// Subsequent calls to StartOutput will invalidate the current handle
        /// and set this to property to null, until a new handle is ready. <see
        /// cref="Vircadia.AudioClient.Update"> Update </see> method must be
        /// called to update this property. Alternatively the <see
        /// cref="Vircadia.AudioClient.OutputReady"> OutputReady </see> event
        /// can be used, retrieve the same handle.
        /// </summary>
        public AudioOutputReader Output { get; private set; }

        internal AudioClient(DomainServer domainServer)
        {
            _context = domainServer.ContextId;
            _inputRequested = false;
            _outputRequested = false;
        }

        private int _context;
        private bool _inputRequested;
        private bool _outputRequested;
        private AudioFormat _inputFormat;
    }

}
