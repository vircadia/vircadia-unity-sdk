//
//  MessagesClient.cs
//  Tests/Runtime
//
//  Created by Nshan G. on 1 Apr 2022.
//  Copyright 2022 Vircadia contributors.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MessagesClient
{
    [UnityTest]
    public IEnumerator ConnectionPasses()
    {

        string testChannel = "Chat";
        string testMessage = "{ \"message\": \"This is Vircadia Unity SDK unit test speaking.\", \"displayName\": \"unitysdk_unit_test\", \"type\":\"TransmitChatMessage\", \"channel\": \"Domain\" }";

        var domainServer = new Vircadia.DomainServer();

        Assert.NotNull(domainServer.Messages);

        domainServer.Connect("localhost");

        bool messageSent = false;
        bool messageReceived = false;
        for (int i = 0; i < 10; ++i)
        {
            var status = domainServer.Status;
            if (status == Vircadia.DomainServerStatus.Connected)
            {
                domainServer.Messages.Enable(Vircadia.MessageType.Text);
                domainServer.Messages.Subscribe(testChannel);

                domainServer.Messages.Update();

                Assert.NotNull(domainServer.Messages.TextMessages);

                foreach (var message in domainServer.Messages.TextMessages)
                {
                    if (message.Channel == testChannel && message.Text == testMessage)
                    {
                        messageReceived = true;
                        break;
                    }
                }

                if (!messageSent)
                {
                    domainServer.Messages.SendTextMessage(testChannel, testMessage);
                    messageSent = true;
                }

            }
            else
            {
                Assert.AreEqual(status, Vircadia.DomainServerStatus.Disconnected);
            }

            yield return new WaitForSeconds(1);
        }

        if (messageSent)
        {
            Assert.IsTrue(messageReceived);
        }

        domainServer.Destroy();

    }
}
