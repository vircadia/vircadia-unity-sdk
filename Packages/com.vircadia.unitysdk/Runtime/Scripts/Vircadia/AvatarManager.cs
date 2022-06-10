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
using System.Collections.Generic;
using UnityEngine;

namespace Vircadia
{

    public struct Joint
    {
        public Vector3? position;
        public Quaternion? rotation;
    }

    public enum AvatarDisconnectReason
    {
        Unknown,
        Normal,
        Ignored,
        TheyEnteredBubble,
        YouEnteredBubble
    };

    /// <summary>
    /// Represents avatar data that can be sent or received.
    /// </summary>
    public struct AvatarData {
        public Guid id;
        public string displayName;
        public bool lookAtSnappingEnabled;
        public string skeletonModelUrl;
        public Vector3 globalPosition;
        public Quaternion orientation;
        public float scale;
        public Bounds bounds;
        public Joint[] pose;
    }

    /// <summary>
    /// Event handler type for handling new avatar connection.
    /// </summary>
    /// <param name="avatar">The newly connected.</param>
    public delegate void AvatarConnecedHandler(Avatar avatar);

    /// <summary>
    /// Event handler type for handling avatar disconnection.
    /// </summary>
    /// <param name="avatar">The disconnected avatar.</param>
    /// <param name="reason">The disconnection reason.</param>
    public delegate void AvatarDisconnectedHandler(Avatar avatar, AvatarDisconnectReason reason);

    /// <summary>
    /// Event handler type for handling avatar updates.
    /// </summary>
    /// <param name="avatar">The updated avatar.</param>
    public delegate void AvatarUpdatedHandler(Avatar avatar);


    /// <summary>
    /// A handle for receiving avatar data from another clients.
    /// </summary>
    public class Avatar {

        /// <summary>
        /// Fires when this avatar is disconnected.
        /// </summary>
        public event AvatarDisconnectedHandler Disconneced = delegate {};

        /// <summary>
        /// Fires whenever this avatar is updated.
        /// </summary>
        public event AvatarUpdatedHandler Updated = delegate {};

        public AvatarData data;

        internal void EmitDisconnect(AvatarDisconnectReason reason)
        {
            Disconneced(this, reason);
        }

        internal void EmitUpdate()
        {
            Updated(this);
        }

        internal bool UpdateData(int index) {

            Guid? id = Utils.getUUID(
                VircadiaNative.Avatars.vircadia_get_avatar_uuid(_context, index));

            string displayName = Marshal.PtrToStringAnsi(
                VircadiaNative.Avatars.vircadia_get_avatar_display_name(_context, index));
            int lookAtSnappingEnabled = VircadiaNative.Avatars.vircadia_get_avatar_look_at_snapping(_context, index);
            string skeletonModelUrl = Marshal.PtrToStringAnsi(
                VircadiaNative.Avatars.vircadia_get_avatar_skeleton_model_url(_context, index));

            VircadiaNative.vector? globalPosition = Marshal.PtrToStructure<VircadiaNative.vector>(
                VircadiaNative.Avatars.vircadia_get_avatar_global_position(_context, index));
            VircadiaNative.quaternion? orientation = Marshal.PtrToStructure<VircadiaNative.quaternion>(
                VircadiaNative.Avatars.vircadia_get_avatar_orientation(_context, index));
            IntPtr scalePtr = VircadiaNative.Avatars.vircadia_get_avatar_scale(_context, index);

            int jointCount = VircadiaNative.Avatars.vircadia_get_avatar_joint_count(_context, index);

            if (id == null || displayName == null || skeletonModelUrl == null || globalPosition == null || orientation == null ||
                scalePtr == IntPtr.Zero || jointCount < 0)
            {
                return false;
            }
            else
            {
                var scale = new float[1];
                Marshal.Copy(scalePtr, scale, 0, 1);

                var joints = new Joint[jointCount];
                for (int i = 0; i < joints.Length; ++i)
                {
                    VircadiaNative.vantage joint = Marshal.PtrToStructure<VircadiaNative.vantage>(
                        VircadiaNative.Avatars.vircadia_get_avatar_joint(_context, index, i));
                    VircadiaNative.joint_flags flags = Marshal.PtrToStructure<VircadiaNative.joint_flags>(
                        VircadiaNative.Avatars.vircadia_get_avatar_joint_flags(_context, index, i));

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

                data.id = id.Value;
                data.displayName = displayName;
                data.skeletonModelUrl = skeletonModelUrl;
                data.globalPosition = new Vector3(globalPosition.Value.x, globalPosition.Value.y, globalPosition.Value.z);
                data.orientation = new Quaternion(orientation.Value.x, orientation.Value.y, orientation.Value.z, orientation.Value.w);
                data.scale = scale[0];
                data.pose = joints;
            }

            return VircadiaNative.Avatars.vircadia_avatar_changed(_context, index) == 1;
        }

        internal Avatar(int context)
        {
            _context = context;
        }

        private int _context;
    }

    /// <summary>
    /// An interface for sending and receiving avatar data.
    /// </summary>
    public class AvatarManager
    {

        /// <summary>
        /// Fires whenever a new avatar is connected. Avatar handling must be
        /// enabled with <see cref="Vircadia.AvatarManager.Enable"> Enable
        /// </see> method and <see cref="Vircadia.AvatarManager.Update"> Update
        /// </see> method needs to be called for these event to fire.
        /// </summary>
        public event AvatarConnecedHandler AvatarConneced = delegate {};

        /// <summary>
        /// Fires whenever an avatar is disconnected. Avatar handling must be
        /// enabled with <see cref="Vircadia.AvatarManager.Enable"> Enable
        /// </see> method and <see cref="Vircadia.AvatarManager.Update"> Update
        /// </see> method needs to be called for these event to fire.
        /// </summary>
        public event AvatarDisconnectedHandler AvatarDisconneced = delegate {};

        /// <summary>
        /// Fires whenever an avatar is updated. Avatar handling must be
        /// enabled with <see cref="Vircadia.AvatarManager.Enable"> Enable
        /// </see> method and <see cref="Vircadia.AvatarManager.Update"> Update
        /// </see> method needs to be called for these event to fire.
        /// </summary>
        public event AvatarUpdatedHandler AvatarUpdated = delegate {};

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
        public List<Avatar> Others
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
            UpdateAvatars();
        }

        /// <summary>
        /// Set this client's avatar data to be sent to the server. The data
        /// will not actually be sent unless the <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> method is
        /// called.
        /// </summary>
        /// <param name="data">The avatar data to send.</param>
        public void Send(AvatarData data)
        {
            IntPtr name = IntPtr.Zero;
            Utils.CreateUnmanaged(ref name, data.displayName);
            VircadiaNative.Avatars.vircadia_set_my_avatar_display_name(_context, name);
            Utils.DestroyUnmanaged(name, data.displayName);

            IntPtr url = IntPtr.Zero;
            Utils.CreateUnmanaged(ref url, data.skeletonModelUrl);
            VircadiaNative.Avatars.vircadia_set_my_avatar_skeleton_model_url(_context, url);
            Utils.DestroyUnmanaged(url, data.skeletonModelUrl);

            VircadiaNative.Avatars.vircadia_set_my_avatar_look_at_snapping(_context,
                (byte) (data.lookAtSnappingEnabled ? 1 : 0));

            VircadiaNative.Avatars.vircadia_set_my_avatar_global_position(_context, new VircadiaNative.vector{
                x = data.globalPosition.x, y = data.globalPosition.y, z = data.globalPosition.z});

            VircadiaNative.Avatars.vircadia_set_my_avatar_orientation(_context, new VircadiaNative.quaternion{
                x = data.orientation.x, y = data.orientation.y, z = data.orientation.z, w = data.orientation.w});

            VircadiaNative.Avatars.vircadia_set_my_avatar_scale(_context, data.scale);

            VircadiaNative.Avatars.vircadia_set_my_avatar_joint_count(_context, data.pose.Length);
            VircadiaNative.Avatars.vircadia_set_my_avatar_joint_flags_count(_context, data.pose.Length);
            for (int i = 0; i < data.pose.Length; ++i)
            {
                var position = data.pose[i].position ?? new Vector3();
                var rotation = data.pose[i].rotation ?? new Quaternion();
                VircadiaNative.Avatars.vircadia_set_my_avatar_joint(_context, i, new VircadiaNative.vantage{
                    position = new VircadiaNative.vector{
                        x = position.x, y = position.y, z = position.z},
                    rotation = new VircadiaNative.quaternion{
                        x = rotation.x, y = rotation.y, z = rotation.z, w = rotation.w}
                });
                VircadiaNative.Avatars.vircadia_set_my_avatar_joint_flags(_context, i, new VircadiaNative.joint_flags{
                    translation_is_default = (byte) (data.pose[i].position == null ? 1 : 0),
                    rotation_is_default = (byte) (data.pose[i].rotation == null ? 1 : 0)
                });
            }

        }

        private int _context;

        internal AvatarManager(DomainServer domainServer)
        {
            _context = domainServer.ContextId;
            Others = new List<Avatar>();
        }

        private void UpdateAvatars()
        {
            int disconnecionCount = VircadiaNative.Avatars.vircadia_get_avatar_disconnection_count(_context);
            if (disconnecionCount < 0)
            {
                return;
            }

            for (int disconnection = 0; disconnection < disconnecionCount; ++disconnection)
            {
                Guid? id = Utils.getUUID(VircadiaNative.Avatars.vircadia_get_avatar_disconnection_uuid(_context, disconnection));
                if (id == null)
                {
                    return;
                }

                int removedIndex = Others.FindIndex(avatar => avatar.data.id == id);
                if (removedIndex != -1)
                {
                    int reason = VircadiaNative.Avatars.vircadia_get_avatar_disconnection_reason(_context, disconnection);
                    if (reason < 0)
                    {
                        continue;
                    }
                    var removed = Others[removedIndex];
                    AvatarDisconneced(removed, (AvatarDisconnectReason) reason);
                    removed.EmitDisconnect((AvatarDisconnectReason) reason);
                    Others.RemoveAt(removedIndex);
                }
            }

            int count = VircadiaNative.Avatars.vircadia_get_avatar_count(_context);
            if (count < 0)
            {
                return;
            }

            for (int i = Others.Count; i < count; ++i)
            {
                var avatar = new Avatar(_context);
                AvatarConneced(avatar);
                Others.Add(avatar);
            }

            for (int i = 0; i < Others.Count; ++i)
            {
                var avatar = Others[i];
                if (avatar.UpdateData(i))
                {
                    AvatarUpdated(avatar);
                    avatar.EmitUpdate();
                }
            }

        }

    }
}
