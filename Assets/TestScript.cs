//
//  Common.cs
//  Scripts/Vircadia/Native
//
//  Created by Nshan G. on 03 Mar 2022.
//  Copyright 2022 Vircadia contributors.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ApplicationInfo
{

    private string _name;
    public string name
    {
        get { return _name ?? Application.productName; }
        set { _name = value; }
    }

    private string _organization;
    public string organization
    {
        get { return _organization ?? Application.companyName; }
        set { _organization = value; }
    }

    public string domain;

    private string _version;
    public string version
    {
        get { return _version ?? Application.version; }
        set { _version = value; }
    }
}

public class TestScript : MonoBehaviour
{
    private Vircadia.DomainServer _domainServer;
    private bool _connected = false;
    private int _nodesSeen = 0;

    public string location = "localhost";
    public ApplicationInfo applicationInfo;

    void Start()
    {
        Debug.Log("Vircadia SDK native API version: " + Vircadia.Info.NativeVersion().full);

        this._domainServer = new Vircadia.DomainServer(
            appName: this.applicationInfo.name,
            appOrganization: this.applicationInfo.organization,
            appDomain: this.applicationInfo.domain,
            appVersion: this.applicationInfo.version);

        if (!string.IsNullOrEmpty(this.location))
        {
            this._domainServer.Connect(this.location);
        }
    }

    void FixedUpdate()
    {
        if (!this._connected)
        {
            Debug.Log("[vircadia-unity-example] Connecting");
            if (this._domainServer.Status == Vircadia.DomainServerStatus.Connected)
            {
                Debug.Log("[vircadia-unity-example] Successfully connected to " + this.location);
                this._connected = true;
            }
        }
        else
        {
            var nodes = this._domainServer.Nodes;
            if (nodes.Length != _nodesSeen)
            {
                Debug.Log("[vircadia-unity-example] Updated node list:");
                foreach (var node in nodes)
                {
                    Debug.Log("[vircadia-unity-example] UUID: " + node.uuid.ToString());
                }
                _nodesSeen = nodes.Length;
            }
        }
    }

}
