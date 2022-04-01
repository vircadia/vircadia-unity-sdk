//
//  Error.cs
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

    }
}
