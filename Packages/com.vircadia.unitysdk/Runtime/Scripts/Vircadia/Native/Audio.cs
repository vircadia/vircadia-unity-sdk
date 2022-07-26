//
//  Audio.cs
//  Runtime/Scripts/Vircadia/Native
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

namespace VircadiaNative
{
    public struct audio_format {
        public byte sample_type;
        public int sample_rate;
        public int channel_count;
    };

    public struct audio_codec_params {
        public byte allowed;
        public byte encoder_vbr;
        public byte encoder_fec;
        public int encoder_bitrate;
        public int encoder_complexity;
        public int encoder_packet_loss;
    };

    public static class Audio
    {

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_enable_audio(int context_id);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_selected_audio_codec_name(int context_id);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_audio_codec_params(int context_id, IntPtr codec, audio_codec_params codec_params);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_audio_input_format(int context_id, audio_format format);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_audio_output_format(int context_id, audio_format format);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_audio_input_context(int context_id);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_audio_output_context(int context_id);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_audio_input_data(IntPtr audio_context, IntPtr data, int size);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_audio_output_data(IntPtr audio_context, IntPtr data, int size);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_audio_bounds(int context_id, bounds bounds);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_audio_vantage(int context_id, vantage vantage);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_audio_input_echo(int context_id, byte enabled);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_audio_output_buffer_frames(int context_id, int frames);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_audio_mixer_injector_gain(int context_id, float gain);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_audio_input_muted(int context_id, byte muted);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_audio_input_muted_by_mixer(int context_id);

    }
}
