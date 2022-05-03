//
//  DomainServer.cs
//  Tests/Runtime
//
//  Created by Nshan G. on 22 Mar 2022.
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

public class DomainServer
{
    [UnityTest]
    public IEnumerator ConnectionPasses()
    {

        var domainServer = new Vircadia.DomainServer();
        domainServer.Connect("localhost");
        for (int i = 0; i < 10; ++i)
        {
            var status = domainServer.Status;
            if (status == Vircadia.DomainServerStatus.Connected)
            {
                var nodes = domainServer.Nodes;
                Assert.AreNotEqual(nodes, null);
                for (int one = 0; one < nodes.Length - 1; ++one)
                {
                    for (int other = one + 1; other < nodes.Length; ++other)
                    {
                        Assert.AreNotEqual(nodes[one].uuid, nodes[other].uuid);
                    }
                }

                if (nodes.Length > 3)
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

        domainServer.Destroy();

    }
}
