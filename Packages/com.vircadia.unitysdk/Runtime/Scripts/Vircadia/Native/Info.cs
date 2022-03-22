//
//  Info.cs
//  Scripts/Vircadia/Native
//
//  Created by Nshan G. on 18 Feb 2022.
//  Copyright 2022 Vircadia contributors.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Runtime.InteropServices;

namespace VircadiaNative
{
    public struct version_data
    {
        public int year;
        public int major;
        public int minor;
        public IntPtr commit;
        public IntPtr number;
        public IntPtr full;
    }

    public static class Info
    {
        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_client_version();
    }
}
