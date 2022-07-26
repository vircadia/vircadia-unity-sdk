//
//  Error.cs
//  Runtime/Scripts/Vircadia/Native
//
//  Created by Nshan G. on 1 Apr 2022.
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

    public static class Error
    {

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_context_exists();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_context_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_context_loss();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_node_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_message_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_message_type_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_message_type_disabled();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_packet_write();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_argument_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_avatars_disabled();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_avatar_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_avatar_attachment_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_avatar_joint_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_avatar_bone_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_avatar_grab_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_avatar_entity_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_avatar_view_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_audio_codec_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_audio_context_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_audio_format_invalid();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_error_audio_disabled();

    }
}
