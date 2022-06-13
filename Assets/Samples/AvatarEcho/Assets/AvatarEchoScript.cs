//
//  AvatarsScript.cs
//  Samples/Avatars
//
//  Created by Nshan G. on 03 Mar 2022.
//  Copyright 2022 Vircadia contributors.
//  Copyright 2022 DigiSomni LLC.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AvatarEchoScript : MonoBehaviour
{
    [System.Serializable]
    public struct MessageData
    {
        public string displayName;
        public string message;
        public string type;
        public string channel;
    }

    private Vircadia.DomainServer _domainServer;
    private bool _connected = false;
    private int _nodesSeen = 0;
    private bool _testMessageSent = false;
    private bool _messagesMixerActive = false;
    private Guid? _echoAvatar;

    public string location = "localhost";

    void Start()
    {
        Debug.Log("Vircadia SDK native API version: " + Vircadia.Info.NativeVersion().full);

        _domainServer = new Vircadia.DomainServer(
            appName: Application.productName,
            appOrganization: Application.companyName,
            appDomain: "",
            appVersion: Application.version);

        if (!string.IsNullOrEmpty(location))
        {
            _domainServer.Connect(location);

            _domainServer.Messages.Enable(Vircadia.MessageType.Text);
            _domainServer.Messages.Subscribe("Chat");

            _domainServer.Avatar.Enable();
        }
    }

    void FixedUpdate()
    {
        if (!_connected)
        {
            Debug.Log("[vircadia-unity-example] Connecting");
            if (_domainServer.Status == Vircadia.DomainServerStatus.Connected)
            {
                Debug.Log("[vircadia-unity-example] Successfully connected to " + location);
                _connected = true;
            }
        }
        else
        {
            var nodes = _domainServer.Nodes;
            if (nodes.Length != _nodesSeen)
            {
                Debug.Log("[vircadia-unity-example] Updated node list:");
                foreach (var node in nodes)
                {
                    Debug.Log("[vircadia-unity-example] UUID: " + node.uuid.ToString());
                    Debug.Log("[vircadia-unity-example] Type: " + node.type.ToString());
                    Debug.Log("[vircadia-unity-example] Active: " + node.active.ToString());
                    Debug.Log("[vircadia-unity-example]");
                }
                _nodesSeen = nodes.Length;
            }

            if (!_messagesMixerActive)
            {
                foreach (var node in nodes)
                {
                    if (node.type == Vircadia.NodeType.MessagesMixer)
                    {
                        _messagesMixerActive = node.active;
                    }
                }
            }


            if (!_testMessageSent && _messagesMixerActive)
            {
                _domainServer.Messages.SendTextMessage("Chat", JsonUtility.ToJson(new MessageData{
                    displayName = "Unity SDK Example",
                    message = "This is Vircadia Unity SDK example speaking.",
                    channel = "Domain",
                    type = "TransmitChatMessage"
                }));
                _testMessageSent = true;
            }

            _domainServer.Messages.Update();

            foreach (var message in _domainServer.Messages.TextMessages)
            {
                var data = JsonUtility.FromJson<MessageData>(message.Text);
                Debug.Log("[vircadia-unity-example] Received a chat message: \n" +
                    "    Sender UUID: " + message.Sender.ToString() + "\n" +
                    "    Display Name: " + data.displayName + "\n" +
                    "    Channel: " + data.channel + "\n" +
                    "    Type: " + data.type + "\n" +
                    "    Text: " + data.message + "\n");

                if (_echoAvatar != null)
                {
                    if (message.Sender == _echoAvatar.Value && data.message == "freeze")
                    {
                        _echoAvatar = null;
                    }
                }
                else if (data.message == "echo me")
                {
                    _echoAvatar = message.Sender;
                }
            }

            _domainServer.Avatar.Update();

            if (_echoAvatar != null)
            {
                bool found = false;
                foreach (var avatar in _domainServer.Avatar.Others)
                {
                    if (_echoAvatar.Value == avatar.data.id)
                    {
                        var data = avatar.data;
                        found = true;
                        data.globalPosition.z += 1 * data.scale;
                        data.displayName += " (echo)";
                        _domainServer.Avatar.Send(data);
                    }
                }

                if (!found)
                {
                    _echoAvatar = null;
                }
            }

        }
    }

    void OnDestroy()
    {
        _domainServer.Destroy();
    }

}
