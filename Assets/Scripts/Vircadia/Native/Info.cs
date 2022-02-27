//
//  Info.cs
//  Scripts/Vircadia/Native
//
//  Created by Nshan G. on 18 Feb 2022.
//  Copyright 2022 Vircadia contributors.
//

using System;
using System.Runtime.InteropServices;

namespace VircadiaNative
{
    public struct version_data
    {
        public int major;
        public int minor;
        public int tweak;
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
