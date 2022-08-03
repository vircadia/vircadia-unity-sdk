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
using UnityEngine;

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

    internal static class DataConversion {

        internal static Vector3 VectorFromNative(VircadiaNative.vector v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        internal static Quaternion QuaternionFromNative(VircadiaNative.quaternion q)
        {
            return new Quaternion(q.x, q.y, q.z, q.w);
        }

        internal static Transform TransformFromNative(VircadiaNative.transform t)
        {
            return new Transform {
                translation = VectorFromNative(t.vantage.position),
                rotation = QuaternionFromNative(t.vantage.rotation),
                scale = t.scale,
            };
        }

        internal static Bounds BoundsFromNative(VircadiaNative.bounds b)
        {
            Vector3 offset = VectorFromNative(b.offset);
            Vector3 dimensions = VectorFromNative(b.dimensions);
            return new Bounds(offset + dimensions/2, dimensions);
        }

        internal static VircadiaNative.vector NativeVectorFrom(Vector3 v)
        {
            return new VircadiaNative.vector {
                x = v.x, y = v.y, z = v.z
            };
        }

        internal static VircadiaNative.quaternion NativeQuaternionFrom(Quaternion q)
        {
            return new VircadiaNative.quaternion {
                x = q.x, y = q.y, z = q.z, w = q.w
            };
        }

        internal static VircadiaNative.transform NativeTransformFrom(Transform t)
        {
            return new VircadiaNative.transform {
                vantage = new VircadiaNative.vantage {
                    position = NativeVectorFrom(t.translation),
                    rotation = NativeQuaternionFrom(t.rotation),
                },
                scale = t.scale
            };
        }

        internal static VircadiaNative.bounds NativeBoundsFrom(Bounds b)
        {
            return new VircadiaNative.bounds {
                dimensions = NativeVectorFrom(b.size),
                offset = NativeVectorFrom(b.min)
            };
        }

    }

}
