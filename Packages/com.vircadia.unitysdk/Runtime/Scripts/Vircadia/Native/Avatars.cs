//
//  Avatars.cs
//  Runtime/Scripts/Vircadia/Native
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

namespace VircadiaNative
{

    public struct vector
    {
        public float x;
        public float y;
        public float z;
    };

    public struct quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
    };

    public struct bounds
    {
        public vector dimensions;
        public vector offset;
    };

    public struct vantage
    {
        public vector position;
        public quaternion rotation;
    };

    public struct transform
    {
        public vantage vantage;
        public float scale;
    };

    public struct avatar_additional_flags
    {
        public byte hand_state;
        public byte key_state;
        public byte has_head_scripted_blendshapes;
        public byte has_procedural_eye_movement;
        public byte has_audio_face_movement;
        public byte has_procedural_eye_face_movement;
        public byte has_procedural_blink_face_movement;
        public byte collides_with_avatars;
        public byte has_priority;
    };

    public struct avatar_parent_info
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DataConstants.RFC4122ByteSize)]
        public byte[] uuid;
        public ushort joint_index;
    };

    public struct avatar_hand_controllers
    {
        public vantage left;
        public vantage right;
    };

    public struct avatar_face_tracker_info
    {
        public float left_eye_blink;
        public float right_eye_blink;
        public float average_loundness;
        public float brow_audio_lift;
        public byte blendshape_coefficients_size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public float[] blendshape_coefficients;
    };

    public struct far_grab_joints
    {
        public vantage left;
        public vantage right;
        public vantage mouse;
    };

    public struct joint_flags
    {
        public byte translation_is_default;
        public byte rotation_is_default;
    };

    public struct avatar_attachment
    {
        public IntPtr model_url;
        public IntPtr joint_name;
        public transform transform;
        public byte is_soft;
    };

    public struct avatar_attachment_result
    {
        public avatar_attachment result;
        public int error;
    };

    public struct avatar_bone
    {
        public byte type;
        public transform default_transform;
        public int index;
        public int parent_index;
        public IntPtr name;
    };

    public struct avatar_bone_result
    {
        public avatar_bone result;
        public int error;
    };

    public struct avatar_grab
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DataConstants.RFC4122ByteSize)]
        public byte[] target_id;
        public int joint_index;
        public vantage offset;
    };

    public struct avatar_grab_result
    {
        public IntPtr id;
        public avatar_grab result;
        public int error;
    };

    public struct conical_view_frustum
    {
        public vector position;
        public vector direction;
        public float angle;
        public float radius;
        public float far_clip;
    };

    public struct view_frustum_corners
    {
        public vector position;
        public float radius;
        public float far_clip;

        public vector near_top_left;
        public vector near_top_right;
        public vector near_bottom_left;
        public vector near_bottom_right;
    };

    public static class Avatars
    {

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_enable_avatars(int context_id);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_update_avatars(int context_id);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_display_name(int context_id, IntPtr display_name);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_is_replicated(int context_id, byte is_replicated);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_look_at_snapping(int context_id, byte look_at_snapping_enabled);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_verification(int context_id, byte verification_failed);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_attachment(int context_id, int attachment_index, avatar_attachment attachment);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_attachment_count(int context_id, int attachment_count);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_attachments(int context_id, IntPtr attachments, int size);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_session_display_name(int context_id, IntPtr session_display_name);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_skeleton_model_url(int context_id, IntPtr skeleton_model_url);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_bone_count(int context_id, int bone_count);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_bone(int context_id, int bone_index, avatar_bone bone);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_skeleton_data(int context_id, IntPtr data, int size);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_global_position(int context_id, vector position);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_bounding_box(int context_id, bounds bounding_box);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_orientation(int context_id, quaternion orientation);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_scale(int context_id, float scale);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_look_at(int context_id, vector position);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_audio_loudness(int context_id, float loudness);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_sensor_to_world(int context_id, transform transform);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_additional_flags(int context_id, avatar_additional_flags flags);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_parent_info(int context_id, avatar_parent_info info);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_local_position(int context_id, vector position);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_hand_controllers(int context_id, avatar_hand_controllers controllers);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_face_tracker_info(int context_id, avatar_face_tracker_info info);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_joint_count(int context_id, int joint_count);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_joint(int context_id, int joint_index, vantage joint);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_joint_data(int context_id, IntPtr joints, int size);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_joint_flags_count(int context_id, int joint_count);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_joint_flags(int context_id, int joint_index, joint_flags flags);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_all_joint_flags(int context_id, IntPtr flags, int size);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_my_avatar_grab_joints(int context_id, far_grab_joints joints);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_my_avatar_grab(int context_id, avatar_grab grab);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_my_avatar_release_grab(int context_id, IntPtr uuid);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_avatar_view_count(int context_id, int view_count);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_avatar_view(int context_id, int view_index, conical_view_frustum view_frustum);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_set_avatar_view_corners(int context_id, int view_index, view_frustum_corners view_frustum);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_avatar_count(int context_id);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_uuid(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_display_name(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_avatar_is_replicated(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_avatar_look_at_snapping(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_avatar_verification_failed(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_avatar_attachment_count(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern avatar_attachment_result vircadia_get_avatar_attachment(int context_id, int avatar_index, int attachment_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_session_display_name(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_skeleton_model_url(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_avatar_bone_count(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern avatar_bone_result vircadia_get_avatar_bone(int context_id, int avatar_index, int bone_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_global_position(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_bounding_box(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_orientation(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_scale(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_look_at_position(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_audio_loudness(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_sensor_to_world(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_additional_flags(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_parent_info(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_local_position(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_hand_controllers(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_face_tracker_info(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_avatar_joint_count(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_joint(int context_id, int avatar_index, int joint_index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_avatar_joint_flags_count(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_joint_flags(int context_id, int avatar_index, int joint_index);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_grab_joints(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_avatar_grabs_count(int context_id, int avatar_index);

        [DllImport(DLLConstants.Import)]
        public static extern avatar_grab_result vircadia_get_avatar_grab(int context_id, int avatar_index, int grab_index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_avatar_disconnection_count(int context_id);

        [DllImport(DLLConstants.Import)]
        public static extern IntPtr vircadia_get_avatar_disconnection_uuid(int context_id, int disconnection_index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_get_avatar_disconnection_reason(int context_id, int disconnection_index);

        [DllImport(DLLConstants.Import)]
        public static extern int vircadia_avatar_changed(int context_id, int disconnection_index);

    }
}
