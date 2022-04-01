//
//  MessageTypes.cs
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

    public static class MessageTypes
    {

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_text_messages();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_data_messages();

        [DllImport(DLLConstants.Import)]
        public static extern byte vircadia_any_messages();

    }
}
