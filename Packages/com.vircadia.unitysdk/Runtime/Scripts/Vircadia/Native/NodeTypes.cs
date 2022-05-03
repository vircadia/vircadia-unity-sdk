//
//  NodeTypes.cs
//  Runtime/Scripts/Vircadia/Native
//
//  Created by Nshan G. on 14 Apr 2022.
//  Copyright 2022 Vircadia contributors.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Runtime.InteropServices;

namespace VircadiaNative
{

    public static class NodeTypes
    {

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_text_messages();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_domain_server_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_entity_server_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_agent_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_audio_mixer_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_avatar_mixer_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_asset_server_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_messages_mixer_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_entity_script_server_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_upstream_audio_mixer_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_upstream_avatar_mixer_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_downstream_audio_mixer_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_downstream_avatar_mixer_node();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_unassigned_node();


    }
}
