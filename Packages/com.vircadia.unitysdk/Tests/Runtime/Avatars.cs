//
//  Avatars.cs
//  Tests/Runtime
//
//  Created by Nshan G. on 12 June 2022.
//  Copyright 2022 Vircadia contributors.
//  Copyright 2022 DigiSomni LLC.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// TODO: to properly test avatars we need two client instances sender and
/// receive, but that currently requires one process per client which is not
/// possible in Unity editor where unit tests run.

public class Avatars
{
    [UnityTest]
    public IEnumerator ConnectionPasses()
    {

        string testName = "Vircadia Unity SDK Avatar Name";

        var domainServer = new Vircadia.DomainServer();

        Assert.NotNull(domainServer.Avatar);
        domainServer.Avatar.Enable();

        domainServer.Connect("localhost");

        bool nameSent = false;
        bool nameReceived = false;
        bool avatarMixerActive = false;
        for (int i = 0; i < 10; ++i)
        {
            var status = domainServer.Status;
            if (status == Vircadia.DomainServerStatus.Connected)
            {
                foreach (var node in domainServer.Nodes)
                {
                    if (node.type == Vircadia.NodeType.AvatarMixer)
                    {
                        avatarMixerActive = node.active;
                    }
                }


                domainServer.Avatar.Update();

                foreach (var avatar in domainServer.Avatar.Others)
                {
                    if (avatar.data.id == domainServer.sessionUUID && avatar.data.displayName == testName)
                    {
                        nameReceived = true;
                    }
                }

                if (!nameSent && avatarMixerActive)
                {
                    domainServer.Avatar.SendDisplayName(testName);
                    nameSent = true;
                }


                if (nameReceived)
                {
                    break;
                }

            }
            else
            {
                Assert.AreEqual(status, Vircadia.DomainServerStatus.Disconnected);
            }

            yield return new WaitForSeconds(1);
        }

        if (nameSent)
        {
            Assert.IsTrue(nameReceived);
        }

        domainServer.Destroy();

    }
}
