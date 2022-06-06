//
//  AvatarManager.cs
//  Runtime/Scripts/Vircadia
//
//  Created by Nshan G. on 6 June 2022.
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

    public struct Joint
    {
        public Vector3? position;
        public Quaternion? rotation;
    }

    /// <summary>
    /// Represents avatar data that can be sent or received.
    /// </summary>
    public struct AvatarData {
        public Guid id;
        public string displayName;
        public string skeletonModelUrl;
        public Vector3 globalPosition;
        public Quaternion orientation;
        public float scale;
        public Bounds bounds;
        // no scale
        public Joint[] joints;
    }

    /// <summary>
    /// A handle for receiving avatar data from another clients.
    /// </summary>
    public class Avatar {
        // TODO: Disconnected/Updated events

        public AvatarData? GetData() {
            Guid? id = Utils.getUUID(
                VircadiaNative.Avatars.vircadia_get_avatar_uuid(_context, _index));
            string displayName = Marshal.PtrToStringAnsi(
                VircadiaNative.Avatars.vircadia_get_avatar_display_name(_context, _index));
            string skeletonModelUrl = Marshal.PtrToStringAnsi(
                VircadiaNative.Avatars.vircadia_get_avatar_skeleton_model_url(_context, _index));
            VircadiaNative.vector? globalPosition = Marshal.PtrToStructure<VircadiaNative.vector>(
                VircadiaNative.Avatars.vircadia_get_avatar_global_position(_context, _index));
            VircadiaNative.quaternion? orientation = Marshal.PtrToStructure<VircadiaNative.quaternion>(
                VircadiaNative.Avatars.vircadia_get_avatar_orientation(_context, _index));
            IntPtr scalePtr = VircadiaNative.Avatars.vircadia_get_avatar_scale(_context, _index);
            int jointCount = VircadiaNative.Avatars.vircadia_get_avatar_joint_count(_context, _index);
            if (id == null || displayName == null || skeletonModelUrl == null || globalPosition == null || orientation == null ||
                scalePtr == IntPtr.Zero || jointCount < 0)
            {
                return null;
            }
            else
            {
                var scale = new float[1];
                Marshal.Copy(scalePtr, scale, 0, 1);

                var joints = new Joint[jointCount];
                for (int i = 0; i < joints.Length; ++i)
                {
                    VircadiaNative.vantage joint = Marshal.PtrToStructure<VircadiaNative.vantage>(
                        VircadiaNative.Avatars.vircadia_get_avatar_joint(_context, _index, i));
                    VircadiaNative.joint_flags flags = Marshal.PtrToStructure<VircadiaNative.joint_flags>(
                        VircadiaNative.Avatars.vircadia_get_avatar_joint_flags(_context, _index, i));

                    if (flags.translation_is_default == 0)
                    {
                        joints[i].position = new Vector3(joint.position.x, joint.position.y, joint.position.z);
                    }
                    else
                    {
                        joints[i].position = null;
                    }

                    if (flags.rotation_is_default == 0)
                    {
                        joints[i].rotation = new Quaternion(joint.rotation.x, joint.rotation.y, joint.rotation.z, joint.rotation.w);
                    }
                    else
                    {
                        joints[i].rotation = null;
                    }

                }

                AvatarData data = new AvatarData();
                data.id = id.Value;
                data.displayName = displayName;
                data.skeletonModelUrl = skeletonModelUrl;
                data.globalPosition = new Vector3(globalPosition.Value.x, globalPosition.Value.y, globalPosition.Value.z);
                data.orientation = new Quaternion(orientation.Value.x, orientation.Value.y, orientation.Value.z, orientation.Value.w);
                data.scale = scale[0];
                data.joints = joints;
                return data;
            }
        }

        internal Avatar(int context, int index)
        {
            _context = context;
            _index = index;
        }

        internal int _index;
        private int _context;
    }

    /// <summary>
    /// Provides APIs for sending and receiving avatar data.
    /// </summary>
    public class AvatarManager
    {

        // TODO: AvatarConnected/Disconnected/Updated events

        /// <summary>
        /// Enables handling of avatars. The incoming and outgoing data is
        /// buffered and the <see cref="Vircadia.AvatarManager.Update"> Update </see>
        /// method must be called periodically to update the internal buffers,
        /// deliver events and send network packets.
        /// </summary>
        public void Enable()
        {
            VircadiaNative.Avatars.vircadia_enable_avatars(_context);
        }

        /// <summary>
        /// The currently buffered avatar data from other clients. These are
        /// updated whenever <see cref="Vircadia.AvatarManager.Update"> Update
        /// </see> method is called.
        /// </summary>
        public Avatar[] Others
        {
            get; private set;
        }

        /// <summary>
        /// Updates the internal avatar data buffers based on data received
        /// from the server, triggers events, and send this client's avatar
        /// data to the server. Avatar handling must be enabled (<see
        /// cref="Vircadia.AvatarManager.Enable"> Enable </see>).
        /// </summary>
        public void Update()
        {
            VircadiaNative.Avatars.vircadia_update_avatars(_context);
            Others = GetAvatars();
        }

        /// <summary>
        /// Set our client's avatar data to be sent to the server. The data
        /// will not actually be sent unless the <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> method is called.
        /// </summary>
        /// <param name="data">The avatar data to send. </param>
        public void Send(AvatarData data)
        {
            IntPtr name = IntPtr.Zero;
            Utils.CreateUnmanaged(ref name, data.displayName);
            IntPtr url = IntPtr.Zero;
            Utils.CreateUnmanaged(ref url, data.skeletonModelUrl);

            VircadiaNative.Avatars.vircadia_set_my_avatar_display_name(_context, name);
            VircadiaNative.Avatars.vircadia_set_my_avatar_skeleton_model_url(_context, url);
            VircadiaNative.Avatars.vircadia_set_my_avatar_global_position(_context, new VircadiaNative.vector{
                x = data.globalPosition.x, y = data.globalPosition.y, z = data.globalPosition.z});
            VircadiaNative.Avatars.vircadia_set_my_avatar_orientation(_context, new VircadiaNative.quaternion{
                x = data.orientation.x, y = data.orientation.y, z = data.orientation.z, w = data.orientation.w});
            VircadiaNative.Avatars.vircadia_set_my_avatar_scale(_context, data.scale);

            VircadiaNative.Avatars.vircadia_set_my_avatar_joint_count(_context, data.joints.Length);
            VircadiaNative.Avatars.vircadia_set_my_avatar_joint_flags_count(_context, data.joints.Length);
            for (int i = 0; i < data.joints.Length; ++i)
            {
                var position = data.joints[i].position ?? new Vector3();
                var rotation = data.joints[i].rotation ?? new Quaternion();
                VircadiaNative.Avatars.vircadia_set_my_avatar_joint(_context, i, new VircadiaNative.vantage{
                    position = new VircadiaNative.vector{
                        x = position.x, y = position.y, z = position.z},
                    rotation = new VircadiaNative.quaternion{
                        x = rotation.x, y = rotation.y, z = rotation.z, w = rotation.w}
                });
                VircadiaNative.Avatars.vircadia_set_my_avatar_joint_flags(_context, i, new VircadiaNative.joint_flags{
                    translation_is_default = (byte) (data.joints[i].position == null ? 1 : 0),
                    rotation_is_default = (byte) (data.joints[i].rotation == null ? 1 : 0)
                });
            }

            Utils.DestroyUnmanaged(name, data.displayName);
            Utils.DestroyUnmanaged(url, data.skeletonModelUrl);
        }

        private int _context;

        internal AvatarManager(DomainServer domainServer)
        {
            _context = domainServer.ContextId;
        }

        private Avatar[] GetAvatars()
        {
            int count = VircadiaNative.Avatars.vircadia_get_avatar_count(_context);
            if (count < 0)
            {
                return null;
            }

            Avatar[] avatars = new Avatar[count];
            for (int i = 0; i < count; ++i)
            {
                avatars[i] = new Avatar(_context, i);
            }

            return avatars;
        }

    }
}
