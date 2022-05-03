//
//  Context.cs
//  Runtime/Scripts/Vircadia/Native
//
//  Created by Nshan G. on 21 Mar 2022.
//  Copyright 2022 Vircadia contributors.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Runtime.InteropServices;

namespace VircadiaNative
{

    public struct client_info
    {
        public IntPtr name;
        public IntPtr organization;
        public IntPtr domain;
        public IntPtr version;
    }

    public struct context_params
    {
        public int listenPort;
        public int dtlsListenPort;
        public IntPtr platform_info;
        public IntPtr user_agent;
        public client_info info;
    }

    public static class Context
    {
        [DllImport(DLLConstants.Import)]
        public static extern context_params vircadia_context_defaults();

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_create_context(context_params _);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_destroy_context(int context);

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
    }
}
