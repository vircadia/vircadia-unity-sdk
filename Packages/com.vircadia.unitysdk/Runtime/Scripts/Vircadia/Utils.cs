//
//  MessagesClient.cs
//  Runtime/Scripts/Vircadia
//
//  Created by Nshan G. on 1 Apr 2022.
//  Copyright 2022 Vircadia contributors.
//  Copyright 2022 DigiSomni LLC.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Runtime.InteropServices;

namespace Vircadia
{
    internal static class Utils
    {
        internal static void CreateUnmanaged(ref IntPtr param, string value)
        {
            if (value != null)
            {
                param = Marshal.StringToCoTaskMemAnsi(value);
            }
        }

        internal static void DestroyUnmanaged(IntPtr param, string value)
        {
            if (value != null)
            {
                Marshal.FreeCoTaskMem(param);
            }
        }

        internal static Guid? getUUID(IntPtr nativeUUID)
        {
            if (nativeUUID == IntPtr.Zero)
            {
                return null;
            }

            byte[] uuidBytes = new byte[VircadiaNative.DataConstants.RFC4122ByteSize];
            Marshal.Copy(nativeUUID, uuidBytes, 0, uuidBytes.Length);
            return new Guid(uuidBytes);
        }

        internal static T? PtrToStructureOrNull<T>(IntPtr ptr)
            where T : struct
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return Marshal.PtrToStructure<T>(ptr);
        }

        internal static float? PtrToFloat(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            var result = new float[1];
            Marshal.Copy(ptr, result, 0, 1);

            return result[0];
        }

    }
}
