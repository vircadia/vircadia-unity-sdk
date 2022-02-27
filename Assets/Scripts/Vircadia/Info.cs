//
//  Info.cs
//  Scripts/Vircadia
//
//  Created by Nshan G. on 18 Feb 2022.
//  Copyright 2022 Vircadia contributors.
//

using System;
using System.Runtime.InteropServices;

namespace Vircadia
{

    public class VersionData
    {
        public int major;
        public int minor;
        public int tweak;
        public string commit;
        public string number;
        public string full;
    }

    public static class Info
    {

        public static VersionData NativeVersion()
        {
            var nativeData = Marshal.PtrToStructure<VircadiaNative.version_data>(VircadiaNative.Info.vircadia_client_version());
            return new VersionData
            {
                major = nativeData.major,
                minor = nativeData.minor,
                tweak = nativeData.tweak,
                commit = Marshal.PtrToStringUTF8(nativeData.commit),
                number = Marshal.PtrToStringUTF8(nativeData.number),
                full = Marshal.PtrToStringUTF8(nativeData.full)
            };
        }

    }
}
