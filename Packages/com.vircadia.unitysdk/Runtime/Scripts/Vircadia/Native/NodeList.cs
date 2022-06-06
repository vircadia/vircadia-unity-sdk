//
//  Client.cs
//  Runtime/Scripts/Vircadia/Native
//
//  Created by Nshan G. on 1 Apr 2022.
//  Copyright 2022 Vircadia contributors.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Runtime.InteropServices;

namespace VircadiaNative
{

    public static class NodeList
    {
        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_connect(int context, IntPtr location);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_connection_status(int context);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_update_nodes(int context);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_node_count(int context);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_node_uuid(int context, int index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_node_active(int context, int index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_node_type(int context, int index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_client_get_session_uuid(int context_id);

    }
}
