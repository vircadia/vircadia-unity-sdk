//
//  Info.cs
//  Runtime/Scripts/Vircadia
//
//  Created by Nshan G. on 18 Feb 2022.
//  Copyright 2022 Vircadia contributors.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
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
        /// The year of the release number, updated with the first release of the year, either minor or major.
        /// </summary>
        public int year;

        /// <summary>
        /// The major version number, increments with API breaking changes.
        /// </summary>
        public int major;

        /// <summary>
        /// The minor version number, increments with backward compatible API changes/additions, bug fixes and implementation changes.
        /// </summary>
        public int minor;

        /// <summary>
        /// Short VCS identifier.
        /// </summary>
        public string commit;

        /// <summary>
        /// Formatted version number string, contains <see cref="P:Vircadia.Info.VersionData.major">major</see> and <see cref="P:Vircadia.Info.VersionData.minor">minor</see>.
        /// </summary>
        public string number;

        /// <summary>
        /// Formatted version string, contains <see cref="P:Vircadia.Info.VersionData.year">year</see>, <see cref="P:Vircadia.Info.VersionData.major">major</see>, <see cref="P:Vircadia.Info.VersionData.minor">minor</see> and <see cref="P:Vircadia.Info.VersionData.commit">commit</see>.
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
                year = nativeData.year,
                major = nativeData.major,
                minor = nativeData.minor,
                commit = Marshal.PtrToStringAnsi(nativeData.commit),
                number = Marshal.PtrToStringAnsi(nativeData.number),
                full = Marshal.PtrToStringAnsi(nativeData.full)
            };
        }

    }
}
