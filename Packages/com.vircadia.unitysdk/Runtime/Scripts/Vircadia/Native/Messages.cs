//
//  Messages.cs
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

    public static class Messages
    {

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_enable_messages(int context_id, byte types);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_messages_subscribe(int context_id, IntPtr channel);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_messages_unsubscribe(int context_id, IntPtr channel);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_update_messages(int context_id, byte types);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_messages_count(int context_id, byte type);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_message(int context_id, byte type, int index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_message_size(int context_id, byte type, int index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_message_channel(int context_id, byte type, int index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_is_message_local_only(int context_id, byte type, int index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_message_sender(int context_id, byte type, int index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_clear_messages(int context_id, byte types);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_send_message(int context_id, byte types, IntPtr channel, IntPtr payload, int size, byte local);

    }
}
