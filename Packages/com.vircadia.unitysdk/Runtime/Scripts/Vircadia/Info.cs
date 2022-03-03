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

    /// <summary>
    /// Represents API version information.
    /// </summary>
    public class VersionData
    {
        /// <summary>
        /// The major version number, increments on compatibility breaking changes.
        /// </summary>
        public int major;

        /// <summary>
        /// The minor version number, increments on backwards compatible API addition or major implementation changes.
        /// </summary>
        public int minor;

        /// <summary>
        /// The tweak version number, increments on bug fixes and minor implementation changes.
        /// </summary>
        public int tweak;

        /// <summary>
        /// Short VCS identifier.
        /// </summary>
        public string commit;

        /// <summary>
        /// Formatted version number string, contains <see cref="P:Vircadia.Info.VersionData.major">major</see>, <see cref="P:Vircadia.Info.VersionData.minor">minor</see> and <see cref="P:Vircadia.Info.VersionData.tweak">tweak</see>.
        /// </summary>
        public string number;

        /// <summary>
        /// Formatted version string, contains <see cref="P:Vircadia.Info.VersionData.major">major</see>, <see cref="P:Vircadia.Info.VersionData.minor">minor</see>, <see cref="P:Vircadia.Info.VersionData.tweak">tweak</see> and <see cref="P:Vircadia.Info.VersionData.commit">commit</see>.
        /// </summary>
        public string full;
    }

    /// <summary>
    /// Provided various auxiliary information about the SDK and native API.
    /// </summary>
    public static class Info
    {

        /// <summary>
        /// Retrieves version of the native C API.
        /// </summary>
        /// <returns> An object containing version information. </returns>
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
