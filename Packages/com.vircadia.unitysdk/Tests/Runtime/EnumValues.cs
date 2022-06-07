//
//  EnumValues.cs
//  Tests/Runtime
//
//  Created by Nshan G. on 14 Apr 2022.
//  Copyright 2022 Vircadia contributors.
//  Copyright 2022 DigiSomni LLC.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using NUnit.Framework;

public class EnumValues
{
    [Test]
    public void MessageTypeValues()
    {
        Assert.AreEqual((byte)Vircadia.MessageType.Text, VircadiaNative.MessageTypes.vircadia_text_messages());
        Assert.AreEqual((byte)Vircadia.MessageType.Data, VircadiaNative.MessageTypes.vircadia_data_messages());
        Assert.AreEqual((byte)Vircadia.MessageType.Any, VircadiaNative.MessageTypes.vircadia_any_messages());
    }

    [Test]
    public void NodeTypeValues()
    {
        Assert.AreEqual((byte)Vircadia.NodeType.DomainServer, VircadiaNative.NodeTypes.vircadia_domain_server_node());
        Assert.AreEqual((byte)Vircadia.NodeType.EntityServer, VircadiaNative.NodeTypes.vircadia_entity_server_node());
        Assert.AreEqual((byte)Vircadia.NodeType.Agent, VircadiaNative.NodeTypes.vircadia_agent_node());
        Assert.AreEqual((byte)Vircadia.NodeType.AudioMixer, VircadiaNative.NodeTypes.vircadia_audio_mixer_node());
        Assert.AreEqual((byte)Vircadia.NodeType.AvatarMixer, VircadiaNative.NodeTypes.vircadia_avatar_mixer_node());
        Assert.AreEqual((byte)Vircadia.NodeType.AssetServer, VircadiaNative.NodeTypes.vircadia_asset_server_node());
        Assert.AreEqual((byte)Vircadia.NodeType.MessagesMixer, VircadiaNative.NodeTypes.vircadia_messages_mixer_node());
        Assert.AreEqual((byte)Vircadia.NodeType.EntityScriptServer, VircadiaNative.NodeTypes.vircadia_entity_script_server_node());
        Assert.AreEqual((byte)Vircadia.NodeType.UpstreamAudioMixer, VircadiaNative.NodeTypes.vircadia_upstream_audio_mixer_node());
        Assert.AreEqual((byte)Vircadia.NodeType.UpstreamAvatarMixer, VircadiaNative.NodeTypes.vircadia_upstream_avatar_mixer_node());
        Assert.AreEqual((byte)Vircadia.NodeType.DownstreamAudioMixer, VircadiaNative.NodeTypes.vircadia_downstream_audio_mixer_node());
        Assert.AreEqual((byte)Vircadia.NodeType.DownstreamAvatarMixer, VircadiaNative.NodeTypes.vircadia_downstream_avatar_mixer_node());
        Assert.AreEqual((byte)Vircadia.NodeType.Unassigned, VircadiaNative.NodeTypes.vircadia_unassigned_node());
    }

}
