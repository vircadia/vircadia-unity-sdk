//
//  DomainServer.cs
//  Runtime/Scripts/Vircadia
//
//  Created by Nshan G. on 21 Mar 2022.
//  Copyright 2022 Vircadia contributors.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Runtime.InteropServices;

namespace Vircadia
{

    public struct Ports
    {
        public int? listenPort;
        public int? dtlsListenPort;
    }


    public struct Node
    {
        public byte type;
        public bool active;
        public string address;
        public Guid uuid;
    }

    public enum DomainServerStatus
    {
        Connected,
        Disconnected,
        Error,
        Unknown
    }

    public class DomainServer
    {

        private int _context;

        static void CreateUnmanaged(ref IntPtr param, string value)
        {
            if (value != null)
            {
                param = Marshal.StringToCoTaskMemUTF8(value);
            }
        }

        static void DestroyUnmanaged(IntPtr param, string value)
        {
            if (value != null)
            {
                Marshal.FreeCoTaskMem(param);
            }
        }

        public DomainServer
        (
            Ports? ports = null,
            string platformInfo = null,
            string userAgent = null,
            string appName = null,
            string appOrganization = null,
            string appDomain = null,
            string appVersion = null
        )
        {
            var parameters = VircadiaNative.Client.vircadia_context_defaults();
            if (ports != null)
            {
                if (ports.Value.listenPort != null)
                {
                    parameters.listenPort = ports.Value.listenPort.Value;
                }

                if (ports.Value.dtlsListenPort != null)
                {
                    parameters.dtlsListenPort = ports.Value.dtlsListenPort.Value;
                }
            }

            CreateUnmanaged(ref parameters.platform_info, platformInfo);
            CreateUnmanaged(ref parameters.user_agent, userAgent);

            this._context = VircadiaNative.Client.vircadia_create_context(parameters);

            DestroyUnmanaged(parameters.platform_info, platformInfo);
            DestroyUnmanaged(parameters.user_agent, userAgent);
        }

        ~DomainServer()
        {
            if (this._context < 0)
            {
                return;
            }

            if (VircadiaNative.Client.vircadia_destroy_context(this._context) < 0)
            {
                // TODO: report error somehow
            }
        }

        public void Connect(string location)
        {
            IntPtr locationPtr = IntPtr.Zero;
            CreateUnmanaged(ref locationPtr, location);
            VircadiaNative.Client.vircadia_connect(this._context, locationPtr);
            DestroyUnmanaged(locationPtr, location);
        }

        public DomainServerStatus Status
        {
            get
            {
                if (this._context < 0)
                {
                    return DomainServerStatus.Error;
                }

                int nativeStatus = VircadiaNative.Client.vircadia_connection_status(this._context);
                if (nativeStatus < 0)
                {
                    return DomainServerStatus.Error;
                }
                else if (nativeStatus == 0)
                {
                    return DomainServerStatus.Disconnected;
                }
                else if (nativeStatus == 1)
                {
                    return DomainServerStatus.Connected;
                }

                return DomainServerStatus.Unknown;
            }
        }

        public Node[] Nodes
        {
            get
            {
                if(VircadiaNative.Client.vircadia_update_nodes(this._context) < 0)
                {
                    return null;
                }

                int count = VircadiaNative.Client.vircadia_node_count(this._context);

                if (count < 0)
                {
                    return null;
                }

                Node[] values = new Node[count];

                byte[] uuidBytes = new byte[16];
                for (int i = 0; i < count ; ++i)
                {
                    IntPtr nativeUUID = VircadiaNative.Client.vircadia_node_uuid(this._context, i);
                    if (nativeUUID == IntPtr.Zero)
                    {
                        return null;
                    }

                    Marshal.Copy(nativeUUID, uuidBytes, 0, uuidBytes.Length);
                    values[i].uuid = new Guid(uuidBytes);
                    // TODO fill other node fields
                }

                return values;
            }
        }

    }
}
