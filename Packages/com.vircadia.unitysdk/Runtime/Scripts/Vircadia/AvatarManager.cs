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

    /// <summary>
    /// Represents a single joint pose.
    /// </summary>
    public struct Joint
    {

        /// <summary>
        /// The position of the joint. Null indicates that the joint is in the
        /// default position.
        /// </summary>
        public Vector3? position;

        /// <summary>
        /// The rotation of the joint. Null indicates that the joint has the
        /// default rotation.
        /// </summary>
        public Quaternion? rotation;
    }

    /// <summary>
    /// A simple representation of linear transformation.
    /// </summary>
    [Serializable]
    public struct Transform
    {
        /// <summary>
        /// The translation component.
        /// </summary>
        public Vector3 translation;

        /// <summary>
        /// The rotation component.
        /// </summary>
        public Quaternion rotation;

        /// <summary>
        /// The scale component.
        /// </summary>
        public float scale;
    }

    /// <summary>
    /// Represents an avatar attachment.
    /// </summary>
    public struct AvatarAttachment
    {
        /// <summary>
        /// The URL to the attachment's model.
        /// </summary>
        public string modelUrl;

        /// <summary>
        /// The name of the avatar skeleton joint the attachment is attached
        /// to.
        /// </summary>
        public string jointName;

        /// <summary>
        /// The linear transformation op the attachment. Null indicates that he
        /// joint is animated and has not fixed in position.
        /// </summary>
        public Transform? transform;
    }

    /// <summary>
    /// Possible avatar skeleton bone types.
    /// </summary>
    public enum BoneType : byte
    {
        // TODO: documentation
        SkeletonRoot,
        SkeletonChild,
        NonSkeletonRoot,
        NonSkeletonChild
    }

    /// <summary>
    /// Represents a single avatar skeleton bone.
    /// </summary>
    public struct Bone
    {
        /// <summary>
        /// The type of the bone.
        /// </summary>
        public BoneType type;

        /// <summary>
        /// The default pose of the bone.
        /// </summary>
        public Transform defaultTransform;

        /// <summary>
        /// The index of the bone.
        /// </summary>
        public int index;

        /// <summary>
        /// The index of the bone's parent bone.
        /// </summary>
        public int parentIndex;

        /// <summary>
        /// The name of the bone.
        /// </summary>
        public string name;
    }

    [Flags]
    internal enum AvatarHandFlags : byte
    {
        LeftPointing = 1 << 0,
        RightPointing = 1 << 1,
        IndexFingerPointing = 1 << 2
    }

    /// <summary>
    /// Represents additional avatar flags.
    /// </summary>
    public struct AvatarAdditionalFlags
    {
        /// <summary>
        /// Indicates that the avatar's left hand is pointing.
        /// This controls where the laser emanates from. If the index
        /// finger is pointing the laser emanates from the tip of that
        /// finger, otherwise it emanates from the palm.
        /// </summary>
        public bool leftHandPointing;

        /// <summary>
        /// Indicates that the avatar's right hand is pointing.
        /// This controls where the laser emanates from. If the index
        /// finger is pointing the laser emanates from the tip of that
        /// finger, otherwise it emanates from the palm.
        /// </summary>
        public bool rightHandPointing;

        /// <summary>
        /// Indicates that the avatar's index finger on the either hand is pointing.
        /// This controls where the laser emanates from. If the index
        /// finger is pointing the laser emanates from the tip of that
        /// finger, otherwise it emanates from the palm.
        /// </summary>
        public bool indexFingerPointing;

        /// <summary>
        /// Indicates that the avatar face movement is controlled by
        /// code/script.
        /// </summary>
        public bool headHasScriptedBlendshapes;

        // TODO: document or mark private
        public bool hasProceduralEyeMovement;

        /// <summary>
        /// Indicates that the avatar's mouth blend shapes animate
        /// automatically based on detected microphone input.
        ///
        /// Set this to true to fully control the mouth facial blend shapes.
        /// </summary>
        public bool hasAudioFaceMovement;

        /// <summary>
        /// Indicates that the facial blend shapes for avatar's eyes adjust
        /// automatically as the eyes move.
        ///
        /// This can be set to true prevent the iris from being obscured by the
        /// upper or lower lids, or to false to have full control of the eye
        /// blend shapes.
        /// </summary>
        public bool hasProceduralEyeFaceMovement;

        /// <summary>
        /// Indicates whether the avatar blinks automatically by animating
        /// facial blend shapes.
        ///
        /// Set to false to to fully control the blink facial blend shapes.
        /// </summary>
        public bool hasProceduralBlinkFaceMovement;

        /// <summary>
        /// Indicates that the avatar can collide with other avatars.
        /// </summary>
        public bool collidesWithAvatars;

        /// <summary>
        /// Read only flag, indicates that the avatar is in "hero" zone.
        /// </summary>
        public bool hasPriority;
    }

    /// <summary>
    /// Represents avatar parent information.
    /// </summary>
    public struct AvatarParentInfo
    {
        /// <summary>
        /// The id of the parent.
        /// </summary>
        public Guid id;

        /// <summary>
        /// The joint index of the parent that the avatars local position is
        /// relative to.
        /// </summary>
        public int jointIndex;
    }

    /// <summary>
    /// Represents position and rotation of the left and right hand controllers.
    /// </summary>
    public struct AvatarHandControllers
    {
        public Vector3 leftPosition;
        public Vector3 rightPosition;
        public Quaternion leftRotation;
        public Quaternion rightRotation;
    }

    /// <summary>
    /// Represents avatar's facial expression.
    /// </summary>
    public struct FaceTrackerInfo
    {
        // TOOD: these 4 are unused in the native interface
        public float leftEyeBlink;
        public float rightEyeBlink;
        public float averageLoudness;
        public float browAudioLift;

        /// <summary>
        /// The blend shape coefficient array that define the avatar's facial
        /// expression.
        /// </summary>
        public float[] blendshapeCoefficients;
    }

    /// <summary>
    /// Represents avatar far grab joint positions and rotations.
    /// </summary>
    public struct AvatarGrabJoints
    {
        public Vector3 leftPosition;
        public Vector3 rightPosition;
        public Vector3 mousePosition;
        public Quaternion leftRotation;
        public Quaternion rightRotation;
        public Quaternion mouseRotation;
    };

    /// <summary>
    /// Represents grab action data.
    /// </summary>
    public struct GrabData
    {
        /// <summary>
        /// The id of the grabbed object.
        /// </summary>
        public Guid target;

        /// <summary>
        /// The index of the joint that the avatar is grabbing with.
        /// </summary>
        public int jointIndex;

        /// <summary>
        /// The position of the grabbed object relative to the grabbing joint.
        /// </summary>
        public Vector3 translation;

        /// <summary>
        /// The rotation of the grabbed object relative to the grabbing joint.
        /// </summary>
        public Quaternion rotation;
    }

    /// <summary>
    /// Represents grab action instance.
    /// </summary>
    public struct GrabAction
    {
        /// <summary>
        /// Represents id the grab action.
        /// </summary>
        public Guid id;

        /// <summary>
        /// The grab action data.
        /// </summary>
        public GrabData data;
    }

    /// <summary>
    /// Represents camera view for culling object data received from the
    /// server.
    /// </summary>
    public struct CameraView
    {

        /// <summary>
        /// The position of the camera.
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// An additional culling radius centered around the camera.
        /// </summary>
        public float radius;

        /// <summary>
        /// The far clip plane's distance from the camera.
        /// </summary>
        public float farClip;

        /// <summary>
        /// The four corners of the camera's near clip plane.
        /// </summary>
        public Vector3[] nearCorners;

        /// <summary>
        /// Constructs a camera view form Unity camera object.
        /// </summary>
        /// <param name="camera">The camera to construct the view from.</param>
        /// <param name="radius">The additional culling radius of the view.</param>
        /// <param name="eye">The camera eye to use.</param>
        public CameraView(Camera camera, float radius = 1, Camera.MonoOrStereoscopicEye eye = Camera.MonoOrStereoscopicEye.Mono)
        {
            position = camera.transform.position;
            farClip = camera.farClipPlane;
            this.radius = radius;
            nearCorners = new Vector3[4];
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.nearClipPlane, eye, nearCorners);
        }
    }

    internal class AvatarUtils
    {
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
            return new Bounds(VectorFromNative(b.offset), VectorFromNative(b.dimensions));
        }

        internal static AvatarAdditionalFlags AdditionalFlagsFromNative(VircadiaNative.avatar_additional_flags f)
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

        internal static AvatarHandControllers HandControllersFromNative(VircadiaNative.avatar_hand_controllers c)
        {
            return new AvatarHandControllers {
                leftPosition = VectorFromNative(c.left.position),
                rightPosition = VectorFromNative(c.right.position),
                leftRotation = QuaternionFromNative(c.left.rotation),
                rightRotation = QuaternionFromNative(c.right.rotation)
            };
        }

        internal static FaceTrackerInfo FaceTrackerInfoFromNative(VircadiaNative.avatar_face_tracker_info f)
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

        internal static AvatarGrabJoints GrabJointsFromNative(VircadiaNative.far_grab_joints j)
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
                offset = NativeVectorFrom(b.center)
            };
        }

        internal static VircadiaNative.avatar_additional_flags NativeAdditionalFlagsFrom(AvatarAdditionalFlags f)
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

        internal static VircadiaNative.avatar_hand_controllers NativeHandControllersFrom(AvatarHandControllers c)
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

        internal static VircadiaNative.avatar_face_tracker_info NativeFaceTrackerInfoFrom(FaceTrackerInfo f)
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

        internal static VircadiaNative.far_grab_joints NativeGrabJointsFrom(AvatarGrabJoints j)
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

    /// <summary>
    /// Possible disconnection reasons for other avatars.
    /// </summary>
    public enum AvatarDisconnectReason
    {
        // TOOD: documentation
        Unknown,
        Network,
        Ignored,
        TheyEnteredBubble,
        YouEnteredBubble
    };

    /// <summary>
    /// Represents main avatar data that can be sent or received.
    /// </summary>
    public struct AvatarData {
        /// <summary>
        /// The identifier of the avatar.
        /// </summary>
        public Guid id;

        /// <summary>
        /// The avatars name displayed to the application users.
        /// </summary>
        public string displayName;

        /// <summary>
        /// Avatar's "look at snapping" flag.
        ///
        /// If enabled make the avatar's eyes snap to look at another avatar's
        /// eyes when the other avatar is in the line of sight and also has
        /// this flag set.
        /// </summary>
        public bool lookAtSnappingEnabled;

        /// <summary>
        /// Indicates that the avatar's model failed identity verification.
        /// </summary>
        public bool verificationFailed;

        /// <summary>
        /// The list of avatar's attachments.
        /// </summary>
        public AvatarAttachment[] attachments;

        /// <summary>
        /// The URL to skeleton and model of the avatar.
        /// </summary>
        public string skeletonModelUrl;

        /// <summary>
        /// The global position of the avatar.
        /// </summary>
        public Vector3 globalPosition;

        /// <summary>
        /// The absolute orientation of the avatar.
        /// </summary>
        public Quaternion orientation;

        /// <summary>
        /// The scale of the avatar.
        /// </summary>
        public float scale;

        /// <summary>
        /// The bounds of the avatar.
        /// </summary>
        public Bounds bounds;

        /// <summary>
        /// The position the avatar is looking at (with no eye tracking usually
        /// the mouse cursor position)
        /// </summary>
        public Vector3 lookAtPosition;

        /// <summary>
        /// The instantaneous loudness of the avatar's audio.
        /// </summary>
        public float audioLoudness;

        /// <summary>
        /// The scale, rotation and translation transform from the user's real
        /// world to the avatar's size, orientation and position in the virtual
        /// world.
        /// </summary>
        public Transform sensorToWorld;

        /// <summary>
        /// Avatar's additional flags.
        /// </summary>
        public AvatarAdditionalFlags additionalFlags;

        /// <summary>
        /// Avatar's parent information.
        /// </summary>
        public AvatarParentInfo parent;

        /// <summary>
        /// Avatar's position relative to it's <see
        /// cref="Vircadia.AvatarData.parent"> parent </see>.
        /// </summary>
        public Vector3 localPosition;

        /// <summary>
        /// The position and rotation of left and right hand controllers.
        /// </summary>
        public AvatarHandControllers handControllers;

        /// <summary>
        /// The facial animation data of the avatar.
        /// </summary>
        public FaceTrackerInfo faceTrackerInfo;

        /// <summary>
        /// The far grab joints of the avatar.
        /// </summary>
        public AvatarGrabJoints grabJoints;

        /// <summary>
        /// The current pose of the avatar.
        /// </summary>
        public Joint[] pose;

        /// <summary>
        /// Avatar's skeleton data.
        /// </summary>
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

        /// <summary>
        /// The main data of the avatar. Read only.
        /// </summary>
        public AvatarData data;

        /// <summary>
        /// The current grab actions of the avatar.
        /// </summary>
        public GrabAction[] GrabActions { get; internal set; }

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
                GrabActions = grabs;
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
        /// Set this client's avatar data send to the server. Sent
        /// asynchronously after an <see cref="Vircadia.AvatarManager.Update">
        /// Update </see> call.
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

        /// <summary>
        /// Set this client's avatar's name to send to the server. Sent
        /// asynchronously after an <see cref="Vircadia.AvatarManager.Update">
        /// Update </see> call. This is not necessarily exactly what other's
        /// will see, the server may sanitize and/or de-duplicate the name,
        /// </summary>
        /// <param name="displayName">The avatar displayName to send.</param>
        public void SendDisplayName(string displayName)
        {
            IntPtr name = IntPtr.Zero;
            Utils.CreateUnmanaged(ref name, displayName);
            VircadiaNative.Avatars.vircadia_set_my_avatar_display_name(_context, name);
            Utils.DestroyUnmanaged(name, displayName);
        }

        /// <summary>
        /// Set this client's avatar's model URL to send to the server. Sent
        /// asynchronously after an <see cref="Vircadia.AvatarManager.Update">
        /// Update </see> call.
        /// </summary>
        /// <param name="displayName">The avatar skeleton and model URL to send.</param>
        public void SendSkeletonModelUrl(string skeletonModelUrl)
        {
            IntPtr url = IntPtr.Zero;
            Utils.CreateUnmanaged(ref url, skeletonModelUrl);
            VircadiaNative.Avatars.vircadia_set_my_avatar_skeleton_model_url(_context, url);
            Utils.DestroyUnmanaged(url, skeletonModelUrl);
        }

        /// <summary>
        /// Set this client's avatar's "look at snapping" flag to send to the
        /// server. Sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call. See <see
        /// cref="Vircadia.AvatarData.lookAtSnappingEnabled">
        /// AvatarData.lookAtSnappingEnabled </see>.
        /// </summary>
        /// <param name="lookAtSnappingEnabled">The flag value to send.</param>
        public void SendLookAtSnapping(bool lookAtSnappingEnabled)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_look_at_snapping(_context,
                (byte) (lookAtSnappingEnabled ? 1 : 0));
        }

        /// <summary>
        /// Set this client's avatar's global position to send to the server.
        /// Sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call.
        /// </summary>
        /// <param name="globalPosition">The global position vector to send.</param>
        public void SendGlobalPosition(Vector3 globalPosition)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_global_position(
                _context, AvatarUtils.NativeVectorFrom(globalPosition));
        }

        /// <summary>
        /// Set this client's avatar's orientation to send to the server. Sent
        /// asynchronously after an <see cref="Vircadia.AvatarManager.Update">
        /// Update </see> call.
        /// </summary>
        /// <param name="orientation">The orientation quaternion to send.</param>
        public void SendOrientation(Quaternion orientation)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_orientation(
                _context, AvatarUtils.NativeQuaternionFrom(orientation));
        }

        /// <summary>
        /// Set this client's avatar's scale to send to the server. Sent
        /// asynchronously after an <see cref="Vircadia.AvatarManager.Update">
        /// Update </see> call.
        /// </summary>
        /// <param name="scale">The scale value to send.</param>
        public void SendScale(float scale)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_scale(_context, scale);
        }

        /// <summary>
        /// Set this client's avatar's bounds to send to the server. Sent
        /// asynchronously after an <see cref="Vircadia.AvatarManager.Update">
        /// Update </see> call.
        /// </summary>
        /// <param name="bounds">The bounds to send.</param>
        public void SendBounds(Bounds bounds)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_bounding_box(
                _context, AvatarUtils.NativeBoundsFrom(bounds));
        }

        /// <summary>
        /// Set this client's avatar's bounds to send to the server. Sent
        /// asynchronously after an <see cref="Vircadia.AvatarManager.Update">
        /// Update </see> call.
        /// </summary>
        /// <param name="bounds">The bounds to send.</param>
        public void SendLookAtPosition(Vector3 lookAtPosition)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_look_at(
                _context, AvatarUtils.NativeVectorFrom(lookAtPosition));
        }

        /// <summary>
        /// Set this client's avatar's audio loudness value to send to the
        /// server. Sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call. See <see
        /// cref="Vircadia.AvatarData.audioLoudness"> AvatarData.audioLoudness
        /// </see>.
        /// </summary>
        /// <param name="audioLoudness">The audio loudness value to send.</param>
        public void SendAudioLoudness(float audioLoudness)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_audio_loudness(_context, audioLoudness);
        }

        /// <summary>
        /// Set this client's avatar's sensor to world coordinate
        /// transformation to send to the server. Sent asynchronously after an
        /// <see cref="Vircadia.AvatarManager.Update"> Update </see> call. See
        /// <see cref="Vircadia.AvatarData.sensorToWorld">
        /// AvatarData.sensorToWorld </see>.
        /// </summary>
        /// <param name="transform">The transform to send.</param>
        public void SendSensorToWorldTreansform(Transform transform)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_sensor_to_world(
                _context, AvatarUtils.NativeTransformFrom(transform));
        }

        /// <summary>
        /// Set this client's avatar's additional flag values to send to the
        /// server. Sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call.
        /// </summary>
        /// <param name="flags">The flags to send.</param>
        public void SendAdditionalFlags(AvatarAdditionalFlags flags)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_additional_flags(
                _context, AvatarUtils.NativeAdditionalFlagsFrom(flags));
        }

        /// <summary>
        /// Set this client's avatar's parent information to send to the
        /// server. Sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call.
        /// </summary>
        /// <param name="parent">The parent information to send.</param>
        public void SendParentInfo(AvatarParentInfo parent)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_parent_info(_context, new VircadiaNative.avatar_parent_info {
                uuid = parent.id.ToByteArray(), joint_index = (ushort) parent.jointIndex});
        }

        /// <summary>
        /// Set this client's avatar's parent relative position to send to the
        /// server. Sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call. See <see
        /// cref="Vircadia.AvatarData.localPosition"> AvatarData.localPosition
        /// </see>.
        /// </summary>
        /// <param name="localPosition">The relative position vector to send.</param>
        public void SendLocalPosition(Vector3 localPosition)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_local_position(
                _context, AvatarUtils.NativeVectorFrom(localPosition));
        }

        /// <summary>
        /// Set the position and rotation of this client's avatar's hand
        /// controllers to send to the server.  Sent asynchronously after an
        /// <see cref="Vircadia.AvatarManager.Update"> Update </see> call.
        /// </see>.
        /// </summary>
        /// <param name="handControllers">The position and rotation data for
        /// left and right controllers.</param>
        public void SendHandControllers(AvatarHandControllers handControllers)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_hand_controllers(
                _context, AvatarUtils.NativeHandControllersFrom(handControllers));
        }

        /// <summary>
        /// Set this client's avatar's facial animation data to send to the
        /// server. Sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call. </see>.
        /// </summary>
        /// <param name="faceTrackerInfo">The facial animation data to send.</param>
        public void SendFaceTrackerInfo(FaceTrackerInfo faceTrackerInfo)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_face_tracker_info(
                _context, AvatarUtils.NativeFaceTrackerInfoFrom(faceTrackerInfo));
        }

        /// <summary>
        /// Set this client's avatar's facial animation data to send to the
        /// server. Sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call. </see>.
        /// </summary>
        /// <param name="faceTrackerInfo">The facial animation data to send.</param>
        public void SendGrabJoints(AvatarGrabJoints grabJoints)
        {
            VircadiaNative.Avatars.vircadia_set_my_avatar_grab_joints(
                _context, AvatarUtils.NativeGrabJointsFrom(grabJoints));
        }

        /// <summary>
        /// Set this client's avatar's current pose data to send to the server.
        /// Sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call. </see>.
        /// </summary>
        /// <param name="pose">The array of joint positions and rotations to send.</param>
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

        /// <summary>
        /// Set this client's avatar's current pose data to send to the server.
        /// Sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call. </see>.
        /// </summary>
        /// <param name="pose">The joint position and rotation array to send.</param>
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

        /// <summary>
        /// Set this client's avatar's skeleton data to send to the server.
        /// Sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call. </see>.
        /// </summary>
        /// <param name="skeleton">The array of bone data to send.</param>
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

        /// <summary>
        /// Creates a new grab action for this client's avatar. The grab
        /// action data is sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call. </see>.
        /// </summary>
        /// <param name="data">The grab data of the action.</param>
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

        /// <summary>
        /// Removes an existing grab action of this client's avatar. The grab
        /// action data is sent asynchronously after an <see
        /// cref="Vircadia.AvatarManager.Update"> Update </see> call. </see>.
        /// </summary>
        /// <param name="grabActionId">The identifier of the action to remove.</param>
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
