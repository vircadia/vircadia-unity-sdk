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

    public struct Transform
    {
        public Vector3 translation;
        public Quaternion rotation;
        public float scale;
    }

    public struct AvatarAttachment
    {
        public string modelUrl;
        public string jointName;
        public Transform? transform;
    }

    public enum BoneType : byte
    {
        SkeletonRoot,
        SkeletonChild,
        NonSkeletonRoot,
        NonSkeletonChild
    }

    public struct Bone
    {
        public BoneType type;
        public Transform defaultTransform;
        public int index;
        public int parentIndex;
        public string name;
    }

    [Flags]
    internal enum AvatarHandFlags : byte
    {
        LeftPointing = 1 << 0,
        RightPointing = 1 << 1,
        IndexFingerPointing = 1 << 2
    }

    public struct AvatarAdditionalFlags
    {
        public bool leftHandPointing;
        public bool rightHandPointing;
        public bool indexFingerPointing;
        public bool headHasScriptedBlendshapes;
        public bool hasProceduralEyeMovement;
        public bool hasAudioFaceMovement;
        public bool hasProceduralEyeFaceMovement;
        public bool hasProceduralBlinkFaceMovement;
        public bool collidesWithAvatars;
        public bool hasPriority;
    }

    public struct AvatarParentInfo
    {
        public Guid id;
        public int jointIndex;
    }

    public struct AvatarHandControllers
    {
        public Vector3 leftPosition;
        public Vector3 rightPosition;
        public Quaternion leftRotation;
        public Quaternion rightRotation;
    }

    public struct FaceTrackerInfo
    {
        public float leftEyeBlink;
        public float rightEyeBlink;
        public float averageLoudness;
        public float browAudioLift;
        public float[] blendshapeCoefficients;
    }

    public struct AvatarGrabJoints
    {
        public Vector3 leftPosition;
        public Vector3 rightPosition;
        public Vector3 mousePosition;
        public Quaternion leftRotation;
        public Quaternion rightRotation;
        public Quaternion mouseRotation;
    };

    public struct GrabData
    {
        public Guid target;
        public int jointIndex;
        public Vector3 translation;
        public Quaternion rotation;
    }

    public struct GrabAction
    {
        public Guid id;
        public GrabData data;
    }

    public struct CameraView
    {
        public Vector3 position;
        public float radius;
        public float farClip;
        public Vector3[] nearCorners;

        public CameraView(Camera camera, float radius = 1, Camera.MonoOrStereoscopicEye eye = Camera.MonoOrStereoscopicEye.Mono)
        {
            position = camera.transform.position;
            farClip = camera.farClipPlane;
            this.radius = radius;
            nearCorners = new Vector3[4];
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.nearClipPlane, eye, nearCorners);
        }
    }

    public class AvatarUtils
    {
        public static Vector3 VectorFromNative(VircadiaNative.vector v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static Quaternion QuaternionFromNative(VircadiaNative.quaternion q)
        {
            return new Quaternion(q.x, q.y, q.z, q.w);
        }

        public static Transform TransformFromNative(VircadiaNative.transform t)
        {
            return new Transform {
                translation = VectorFromNative(t.vantage.position),
                rotation = QuaternionFromNative(t.vantage.rotation),
                scale = t.scale,
            };
        }

        public static Bounds BoundsFromNative(VircadiaNative.bounds b)
        {
            return new Bounds(VectorFromNative(b.offset), VectorFromNative(b.dimensions));
        }

        public static AvatarAdditionalFlags AdditionalFlagsFromNative(VircadiaNative.avatar_additional_flags f)
        {
            return new AvatarAdditionalFlags {
                leftHandPointing = (f.hand_state & (byte) AvatarHandFlags.LeftPointing) != 0,
                rightHandPointing = (f.hand_state & (byte) AvatarHandFlags.RightPointing) != 0,
                indexFingerPointing = (f.hand_state & (byte) AvatarHandFlags.IndexFingerPointing) != 0,
                headHasScriptedBlendshapes = f.has_head_scripted_blendshapes == 1,
                hasProceduralEyeMovement = f.has_procedural_eye_movement == 1,
                hasAudioFaceMovement = f.has_audio_face_movement == 1,
                hasProceduralEyeFaceMovement = f.has_procedural_eye_face_movement == 1,
                hasProceduralBlinkFaceMovement = f.has_procedural_eye_face_movement == 1,
                collidesWithAvatars = f.collides_with_avatars == 1,
                hasPriority = f.has_priority == 1
            };
        }

        public static AvatarHandControllers HandControllersFromNative(VircadiaNative.avatar_hand_controllers c)
        {
            return new AvatarHandControllers {
                leftPosition = VectorFromNative(c.left.position),
                rightPosition = VectorFromNative(c.right.position),
                leftRotation = QuaternionFromNative(c.left.rotation),
                rightRotation = QuaternionFromNative(c.right.rotation)
            };
        }

        public static FaceTrackerInfo FaceTrackerInfoFromNative(VircadiaNative.avatar_face_tracker_info f)
        {
            var blendshapeCoefficients = new float[f.blendshape_coefficients_size];
            Array.Copy(f.blendshape_coefficients, blendshapeCoefficients, blendshapeCoefficients.Length);
            return new FaceTrackerInfo {
                leftEyeBlink = f.left_eye_blink,
                rightEyeBlink = f.right_eye_blink,
                averageLoudness = f.average_loundness,
                browAudioLift = f.brow_audio_lift,
                blendshapeCoefficients = blendshapeCoefficients
            };
        }

        public static AvatarGrabJoints GrabJointsFromNative(VircadiaNative.far_grab_joints j)
        {
            return new AvatarGrabJoints {
                leftPosition = VectorFromNative(j.left.position),
                rightPosition = VectorFromNative(j.right.position),
                mousePosition = VectorFromNative(j.mouse.position),
                leftRotation = QuaternionFromNative(j.left.rotation),
                rightRotation = QuaternionFromNative(j.right.rotation),
                mouseRotation = QuaternionFromNative(j.mouse.rotation),
            };
        }

        public static VircadiaNative.vector NativeVectorFrom(Vector3 v)
        {
            return new VircadiaNative.vector {
                x = v.x, y = v.y, z = v.z
            };
        }

        public static VircadiaNative.quaternion NativeQuaternionFrom(Quaternion q)
        {
            return new VircadiaNative.quaternion {
                x = q.x, y = q.y, z = q.z, w = q.w
            };
        }

        public static VircadiaNative.transform NativeTransformFrom(Transform t)
        {
            return new VircadiaNative.transform {
                vantage = new VircadiaNative.vantage {
                    position = NativeVectorFrom(t.translation),
                    rotation = NativeQuaternionFrom(t.rotation),
                },
                scale = t.scale
            };
        }

        public static VircadiaNative.bounds NativeBoundsFrom(Bounds b)
        {
            return new VircadiaNative.bounds {
                dimensions = NativeVectorFrom(b.size),
                offset = NativeVectorFrom(b.center)
            };
        }

        public static VircadiaNative.avatar_additional_flags NativeAdditionalFlagsFrom(AvatarAdditionalFlags f)
        {
            byte handState = 0;
            if (f.leftHandPointing)
            {
                handState |= (byte) AvatarHandFlags.LeftPointing;
            }

            if (f.rightHandPointing)
            {
                handState |= (byte) AvatarHandFlags.RightPointing;
            }

            if (f.indexFingerPointing)
            {
                handState |= (byte) AvatarHandFlags.IndexFingerPointing;
            }

            return new VircadiaNative.avatar_additional_flags {
                hand_state = handState,
                key_state = (byte) 0,
                has_head_scripted_blendshapes = (byte) (f.headHasScriptedBlendshapes ? 1 : 0),
                has_procedural_eye_movement = (byte) (f.hasProceduralEyeMovement ? 1 : 0),
                has_audio_face_movement = (byte) (f.hasAudioFaceMovement ? 1 : 0),
                has_procedural_eye_face_movement = (byte) (f.hasProceduralEyeFaceMovement ? 1 : 0),
                has_procedural_blink_face_movement = (byte) (f.hasProceduralBlinkFaceMovement ? 1 : 0),
                collides_with_avatars = (byte) (f.collidesWithAvatars ? 1 : 0),
                has_priority = (byte) (f.hasPriority ? 1 : 0)
            };
        }

        public static VircadiaNative.avatar_hand_controllers NativeHandControllersFrom(AvatarHandControllers c)
        {
            return new VircadiaNative.avatar_hand_controllers {
                left = new VircadiaNative.vantage {
                    position = NativeVectorFrom(c.leftPosition),
                    rotation = NativeQuaternionFrom(c.leftRotation),
                },
                right = new VircadiaNative.vantage {
                    position = NativeVectorFrom(c.rightPosition),
                    rotation = NativeQuaternionFrom(c.rightRotation),
                }
            };
        }

        public static VircadiaNative.avatar_face_tracker_info NativeFaceTrackerInfoFrom(FaceTrackerInfo f)
        {
            var blendshapeCoefficients = new float[256];
            Array.Copy(f.blendshapeCoefficients, blendshapeCoefficients, f.blendshapeCoefficients.Length);
            return new VircadiaNative.avatar_face_tracker_info {
                left_eye_blink = f.leftEyeBlink,
                right_eye_blink = f.rightEyeBlink,
                average_loundness = f.averageLoudness,
                brow_audio_lift = f.browAudioLift,
                blendshape_coefficients_size = (byte) blendshapeCoefficients.Length,
                blendshape_coefficients = blendshapeCoefficients
            };
        }

        public static VircadiaNative.far_grab_joints NativeGrabJointsFrom(AvatarGrabJoints j)
        {
            return new VircadiaNative.far_grab_joints {
                left = new VircadiaNative.vantage {
                    position = NativeVectorFrom(j.leftPosition),
                    rotation = NativeQuaternionFrom(j.leftRotation),
                },
                right = new VircadiaNative.vantage {
                    position = NativeVectorFrom(j.rightPosition),
                    rotation = NativeQuaternionFrom(j.rightRotation),
                },
                mouse = new VircadiaNative.vantage {
                    position = NativeVectorFrom(j.mousePosition),
                    rotation = NativeQuaternionFrom(j.mouseRotation),
                }
            };
        }

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
        public bool verificationFailed;
        public AvatarAttachment[] attachments;
        public string skeletonModelUrl;
        public Vector3 globalPosition;
        public Quaternion orientation;
        public float scale;
        public Bounds bounds;
        public Vector3 lookAtPosition;
        public float audioLoudness;
        public Transform sensorToWorld;
        public AvatarAdditionalFlags additionalFlags;
        public AvatarParentInfo parent;
        public Vector3 localPosition;
        public AvatarHandControllers handControllers;
        public FaceTrackerInfo faceTrackerInfo;
        public AvatarGrabJoints grabJoints;
        public Joint[] pose;
        public Bone[] skeleton;
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
        public GrabAction[] grabActions;

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

            int verificationFailed = VircadiaNative.Avatars.vircadia_get_avatar_verification_failed(_context, index);

            string skeletonModelUrl = Marshal.PtrToStringAnsi(
                VircadiaNative.Avatars.vircadia_get_avatar_skeleton_model_url(_context, index));

            VircadiaNative.vector? globalPosition = Utils.PtrToStructureOrNull<VircadiaNative.vector>(
                VircadiaNative.Avatars.vircadia_get_avatar_global_position(_context, index));

            VircadiaNative.quaternion? orientation = Utils.PtrToStructureOrNull<VircadiaNative.quaternion>(
                VircadiaNative.Avatars.vircadia_get_avatar_orientation(_context, index));

            float? scale = Utils.PtrToFloat(VircadiaNative.Avatars.vircadia_get_avatar_scale(_context, index));

            VircadiaNative.bounds? bounds = Utils.PtrToStructureOrNull<VircadiaNative.bounds>(
                VircadiaNative.Avatars.vircadia_get_avatar_bounding_box(_context, index));

            VircadiaNative.vector? lookAtPosition = Utils.PtrToStructureOrNull<VircadiaNative.vector>(
                VircadiaNative.Avatars.vircadia_get_avatar_look_at_position(_context, index));

            float? audioLoudness = Utils.PtrToFloat(VircadiaNative.Avatars.vircadia_get_avatar_audio_loudness(_context, index));

            VircadiaNative.transform? sensorToWorld = Utils.PtrToStructureOrNull<VircadiaNative.transform>(
                VircadiaNative.Avatars.vircadia_get_avatar_sensor_to_world(_context, index));

            VircadiaNative.avatar_additional_flags? additionalFlags =
                Utils.PtrToStructureOrNull<VircadiaNative.avatar_additional_flags>(
                    VircadiaNative.Avatars.vircadia_get_avatar_additional_flags(_context, index));

            VircadiaNative.avatar_parent_info? parentInfo =
                Utils.PtrToStructureOrNull<VircadiaNative.avatar_parent_info>(
                    VircadiaNative.Avatars.vircadia_get_avatar_parent_info(_context, index));

            VircadiaNative.vector? localPosition = Utils.PtrToStructureOrNull<VircadiaNative.vector>(
                VircadiaNative.Avatars.vircadia_get_avatar_local_position(_context, index));

            VircadiaNative.avatar_hand_controllers? handControllers =
                Utils.PtrToStructureOrNull<VircadiaNative.avatar_hand_controllers>(
                    VircadiaNative.Avatars.vircadia_get_avatar_hand_controllers(_context, index));

            VircadiaNative.avatar_face_tracker_info? faceTrackerInfo =
                Utils.PtrToStructureOrNull<VircadiaNative.avatar_face_tracker_info>(
                    VircadiaNative.Avatars.vircadia_get_avatar_face_tracker_info(_context, index));

            VircadiaNative.far_grab_joints? grabJoints = Utils.PtrToStructureOrNull<VircadiaNative.far_grab_joints>(
                VircadiaNative.Avatars.vircadia_get_avatar_grab_joints(_context, index));

            int jointCount = VircadiaNative.Avatars.vircadia_get_avatar_joint_count(_context, index);

            int attachmentCount = VircadiaNative.Avatars.vircadia_get_avatar_attachment_count(_context, index);

            int boneCount = VircadiaNative.Avatars.vircadia_get_avatar_bone_count(_context, index);

            int grabCount = VircadiaNative.Avatars.vircadia_get_avatar_grabs_count(_context, index);

            if (id == null || displayName == null || skeletonModelUrl == null || globalPosition == null || orientation == null ||
                scale == null || jointCount < 0 || lookAtSnappingEnabled < 0 || verificationFailed < 0 || attachmentCount < 0 ||
                boneCount < 0 || bounds == null || lookAtPosition == null || audioLoudness == null || sensorToWorld == null ||
                additionalFlags == null || parentInfo == null || localPosition == null || handControllers == null ||
                faceTrackerInfo == null || grabJoints == null || grabCount < 0)
            {
                return false;
            }
            else
            {
                var joints = new Joint[jointCount];
                for (int i = 0; i < joints.Length; ++i)
                {
                    VircadiaNative.vantage joint = Marshal.PtrToStructure<VircadiaNative.vantage>(
                        VircadiaNative.Avatars.vircadia_get_avatar_joint(_context, index, i));
                    VircadiaNative.joint_flags flags = Marshal.PtrToStructure<VircadiaNative.joint_flags>(
                        VircadiaNative.Avatars.vircadia_get_avatar_joint_flags(_context, index, i));

                    if (flags.translation_is_default == 0)
                    {
                        joints[i].position = AvatarUtils.VectorFromNative(joint.position);
                    }
                    else
                    {
                        joints[i].position = null;
                    }

                    if (flags.rotation_is_default == 0)
                    {
                        joints[i].rotation = AvatarUtils.QuaternionFromNative(joint.rotation);
                    }
                    else
                    {
                        joints[i].rotation = null;
                    }

                }

                var attachments = new AvatarAttachment[attachmentCount];
                for (int i = 0; i < attachments.Length; ++i)
                {
                    VircadiaNative.avatar_attachment attachment =
                        VircadiaNative.Avatars.vircadia_get_avatar_attachment(_context, index, i).result;
                    attachments[i].modelUrl = Marshal.PtrToStringAnsi(attachment.model_url);
                    attachments[i].jointName = Marshal.PtrToStringAnsi(attachment.joint_name);
                    if (attachment.is_soft == 0)
                    {
                        attachments[i].transform = AvatarUtils.TransformFromNative(attachment.transform);
                    }
                }

                var bones = new Bone[boneCount];
                for (int i = 0; i < bones.Length; ++i)
                {
                    VircadiaNative.avatar_bone bone =
                        VircadiaNative.Avatars.vircadia_get_avatar_bone(_context, index, i).result;
                    bones[i].type = (BoneType) bone.type;
                    bones[i].defaultTransform = AvatarUtils.TransformFromNative(bone.default_transform);
                    bones[i].index = bone.index;
                    bones[i].parentIndex = bone.parent_index;
                    bones[i].name = Marshal.PtrToStringAnsi(bone.name);
                }

                var grabs = new GrabAction[grabCount];
                for (int i = 0; i < grabs.Length; ++i)
                {
                    VircadiaNative.avatar_grab_result grab =
                        VircadiaNative.Avatars.vircadia_get_avatar_grab(_context, index, i);
                    grabs[i].id = Utils.getUUID(grab.id).Value;
                    grabs[i].data.target = new Guid(grab.result.target_id);
                    grabs[i].data.jointIndex = grab.result.joint_index;
                    grabs[i].data.translation = AvatarUtils.VectorFromNative(grab.result.offset.position);
                    grabs[i].data.rotation = AvatarUtils.QuaternionFromNative(grab.result.offset.rotation);
                }

                data.id = id.Value;
                data.displayName = displayName;
                data.lookAtSnappingEnabled = lookAtSnappingEnabled == 1;
                data.verificationFailed = verificationFailed == 1;
                data.attachments = attachments;
                data.skeletonModelUrl = skeletonModelUrl;
                data.globalPosition = AvatarUtils.VectorFromNative(globalPosition.Value);
                data.orientation = AvatarUtils.QuaternionFromNative(orientation.Value);
                data.scale = scale.Value;
                data.bounds = AvatarUtils.BoundsFromNative(bounds.Value);
                data.lookAtPosition = AvatarUtils.VectorFromNative(lookAtPosition.Value);
                data.audioLoudness = audioLoudness.Value;
                data.sensorToWorld = AvatarUtils.TransformFromNative(sensorToWorld.Value);
                data.additionalFlags = AvatarUtils.AdditionalFlagsFromNative(additionalFlags.Value);
                data.parent = new AvatarParentInfo { id = new Guid(parentInfo.Value.uuid), jointIndex = parentInfo.Value.joint_index };
                data.localPosition = AvatarUtils.VectorFromNative(localPosition.Value);
                data.handControllers = AvatarUtils.HandControllersFromNative(handControllers.Value);
                data.faceTrackerInfo = AvatarUtils.FaceTrackerInfoFromNative(faceTrackerInfo.Value);
                data.grabJoints = AvatarUtils.GrabJointsFromNative(grabJoints.Value);
                data.pose = joints;
                data.skeleton = bones;
                grabActions = grabs;
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
        /// The camera views to send to avatar mixer for culling. Sent
        /// asynchronously after an <see cref="Vircadia.AvatarManager.Update">
        /// Update </see> call.
        /// </summary>
        public CameraView[] cameraViews;

        /// <summary>
        /// Updates the internal avatar data buffers based on data received
        /// from the server, triggers events, and sends this client's avatar
        /// data to the server. Avatar handling must be enabled (<see
        /// cref="Vircadia.AvatarManager.Enable"> Enable </see>).
        /// </summary>
        public void Update()
        {
            QueryAvatars();
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
            SendDisplayName(data.displayName);
            SendSkeletonModelUrl(data.skeletonModelUrl);
            SendLookAtSnapping(data.lookAtSnappingEnabled);
            SendGlobalPosition(data.globalPosition);
            SendOrientation(data.orientation);
            SendScale(data.scale);
            SendBounds(data.bounds);
            SendLookAtPosition(data.lookAtPosition);
            SendAudioLoudness(data.audioLoudness);
            SendSensorToWorldTreansform(data.sensorToWorld);
            SendAdditionalFlags(data.additionalFlags);
            SendParentInfo(data.parent);
            SendLocalPosition(data.localPosition);
            SendHandControllers(data.handControllers);
            SendFaceTrackerInfo(data.faceTrackerInfo);
            SendGrabJoints(data.grabJoints);
            SendPose(data.pose);
            SendAttachments(data.attachments);
            SendSkeleton(data.skeleton);
        }

        public void SendDisplayName(string displayName)
        {
            IntPtr name = IntPtr.Zero;
            Utils.CreateUnmanaged(ref name, displayName);
            VircadiaNative.Avatars.vircadia_set_my_avatar_display_name(_context, name);
            Utils.DestroyUnmanaged(name, displayName);
        }

        public void SendSkeletonModelUrl(string skeletonModelUrl)
        {
            IntPtr url = IntPtr.Zero;
            Utils.CreateUnmanaged(ref url, skeletonModelUrl);
            VircadiaNative.Avatars.vircadia_set_my_avatar_skeleton_model_url(_context, url);
            Utils.DestroyUnmanaged(url, skeletonModelUrl);
        }

        public void SendLookAtSnapping(bool lookAtSnappingEnabled)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_look_at_snapping(_context,
                (byte) (lookAtSnappingEnabled ? 1 : 0));
        }

        public void SendGlobalPosition(Vector3 globalPosition)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_global_position(
                _context, AvatarUtils.NativeVectorFrom(globalPosition));
        }

        public void SendOrientation(Quaternion orientation)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_orientation(
                _context, AvatarUtils.NativeQuaternionFrom(orientation));
        }

        public void SendScale(float scale)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_scale(_context, scale);
        }

        public void SendBounds(Bounds bounds)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_bounding_box(
                _context, AvatarUtils.NativeBoundsFrom(bounds));
        }

        public void SendLookAtPosition(Vector3 lookAtPosition)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_look_at(
                _context, AvatarUtils.NativeVectorFrom(lookAtPosition));
        }

        public void SendAudioLoudness(float audioLoudness)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_audio_loudness(_context, audioLoudness);
        }

        public void SendSensorToWorldTreansform(Transform transform)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_sensor_to_world(
                _context, AvatarUtils.NativeTransformFrom(transform));
        }

        public void SendAdditionalFlags(AvatarAdditionalFlags flags)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_additional_flags(
                _context, AvatarUtils.NativeAdditionalFlagsFrom(flags));
        }

        public void SendParentInfo(AvatarParentInfo parent)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_parent_info(_context, new VircadiaNative.avatar_parent_info {
                uuid = parent.id.ToByteArray(), joint_index = (ushort) parent.jointIndex});
        }

        public void SendLocalPosition(Vector3 localPosition)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_local_position(
                _context, AvatarUtils.NativeVectorFrom(localPosition));
        }

        public void SendHandControllers(AvatarHandControllers handControllers)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_hand_controllers(
                _context, AvatarUtils.NativeHandControllersFrom(handControllers));
        }

        public void SendFaceTrackerInfo(FaceTrackerInfo faceTrackerInfo)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_face_tracker_info(
                _context, AvatarUtils.NativeFaceTrackerInfoFrom(faceTrackerInfo));
        }

        public void SendGrabJoints(AvatarGrabJoints grabJoints)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_grab_joints(
                _context, AvatarUtils.NativeGrabJointsFrom(grabJoints));
        }

        public void SendPose(Joint[] pose)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_joint_count(_context, pose.Length);
            VircadiaNative.Avatars.vircadia_set_my_avatar_joint_flags_count(_context, pose.Length);
            for (int i = 0; i < pose.Length; ++i)
            {
                var position = pose[i].position ?? new Vector3();
                var rotation = pose[i].rotation ?? new Quaternion();
                VircadiaNative.Avatars.vircadia_set_my_avatar_joint(_context, i, new VircadiaNative.vantage{
                    position = new VircadiaNative.vector{
                        x = position.x, y = position.y, z = position.z},
                    rotation = new VircadiaNative.quaternion{
                        x = rotation.x, y = rotation.y, z = rotation.z, w = rotation.w}
                });
                VircadiaNative.Avatars.vircadia_set_my_avatar_joint_flags(_context, i, new VircadiaNative.joint_flags{
                    translation_is_default = (byte) (pose[i].position == null ? 1 : 0),
                    rotation_is_default = (byte) (pose[i].rotation == null ? 1 : 0)
                });
            }
        }

        public void SendAttachments(AvatarAttachment[] attachments)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_attachment_count(_context, attachments.Length);
            for (int i = 0; i < attachments.Length; ++i)
            {
                var attachment = attachments[i];

                IntPtr modelUrl = IntPtr.Zero;
                Utils.CreateUnmanaged(ref modelUrl, attachment.modelUrl);

                IntPtr jointName = IntPtr.Zero;
                Utils.CreateUnmanaged(ref jointName, attachment.jointName);

                var transform = attachments[i].transform ?? new Transform();
                VircadiaNative.Avatars.vircadia_set_my_avatar_attachment(_context, i, new VircadiaNative.avatar_attachment{
                    model_url = modelUrl,
                    joint_name = jointName,
                    transform = AvatarUtils.NativeTransformFrom(transform),
                    is_soft = (byte) (attachments[i].transform == null ? 1 : 0)
                });

                Utils.DestroyUnmanaged(modelUrl, attachment.modelUrl);
                Utils.DestroyUnmanaged(jointName, attachment.jointName);
            }
        }

        public void SendSkeleton(Bone[] skeleton)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_bone_count(_context, skeleton.Length);
            for (int i = 0; i < skeleton.Length; ++i)
            {
                var bone = skeleton[i];

                IntPtr namePtr = IntPtr.Zero;
                Utils.CreateUnmanaged(ref namePtr, bone.name);

                VircadiaNative.Avatars.vircadia_set_my_avatar_bone(_context, i, new VircadiaNative.avatar_bone{
                    type = (byte) bone.type,
                    default_transform = AvatarUtils.NativeTransformFrom(bone.defaultTransform),
                    index = bone.index,
                    parent_index = bone.parentIndex,
                    name = namePtr
                });

                Utils.DestroyUnmanaged(namePtr, bone.name);
            }
        }

        public void Grab(GrabData data)
        {
            VircadiaNative.Avatars.vircadia_my_avatar_grab(_context, new VircadiaNative.avatar_grab {
                target_id = data.target.ToByteArray(),
                joint_index = data.jointIndex,
                offset = new VircadiaNative.vantage {
                    position = AvatarUtils.NativeVectorFrom(data.translation),
                    rotation = AvatarUtils.NativeQuaternionFrom(data.rotation)
                }
            });
        }

        public void ReleaseGrab(Guid grabActionId)
        {
            var uuid = grabActionId.ToByteArray();
            unsafe
            {
                fixed (byte* ptr = uuid)
                {
                    VircadiaNative.Avatars.vircadia_my_avatar_release_grab(_context, (IntPtr)ptr);
                }
            }
        }

        private int _context;

        internal AvatarManager(DomainServer domainServer)
        {
            _context = domainServer.ContextId;
            Others = new List<Avatar>();
        }

        internal void QueryAvatars()
        {
            if (cameraViews == null)
            {
                return;
            }

            VircadiaNative.Avatars.vircadia_set_avatar_view_count(_context, cameraViews.Length);
            for (int i = 0; i < cameraViews.Length; ++i)
            {
                VircadiaNative.Avatars.vircadia_set_avatar_view_corners(_context, i, new VircadiaNative.view_frustum_corners {
                    position = AvatarUtils.NativeVectorFrom(cameraViews[i].position),
                    radius = cameraViews[i].radius,
                    far_clip = cameraViews[i].farClip,
                    near_top_left = AvatarUtils.NativeVectorFrom(cameraViews[i].nearCorners[0]),
                    near_top_right = AvatarUtils.NativeVectorFrom(cameraViews[i].nearCorners[1]),
                    near_bottom_right = AvatarUtils.NativeVectorFrom(cameraViews[i].nearCorners[2]),
                    near_bottom_left = AvatarUtils.NativeVectorFrom(cameraViews[i].nearCorners[3]),
                });
            }
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
