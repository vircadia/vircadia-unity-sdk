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

    /// <summary>
    /// Optional fixed ports data.
    /// </summary>
    [System.Serializable]
    public struct Ports
    {
        /// <summary>
        /// UDP port for the client to listen on. (default: random port)
        /// </summary>
        public int? listenPort;

        /// <summary>
        /// DTLS port for the client to listen on. (default: not used)
        /// </summary>
        public int? dtlsListenPort;
    }

    /// <summary>
    /// Node (assignment client) data.
    /// </summary>
    public struct Node
    {
        /// <summary>
        /// Type of the node.
        /// </summary>
        public byte type;

        /// <summary>
        /// Indicated weather the node is active, or not.
        /// </summary>
        public bool active;

        /// <summary>
        /// A human readable address/port representation.
        /// </summary>
        public string address;

        /// <summary>
        /// A universal unique ID of the node (the string representation might not match between implementations)
        /// </summary>
        public Guid uuid;
    }

    /// <summary>
    /// Possible states of the <see cref="P:Vircadia.Client.DomainServer">DomainServer</see> class.
    /// </summary>
    public enum DomainServerStatus
    {
        /// <summary>
        /// Successfully Connected to a domain server, specified with <see cref="P:Vircadia.Client.DomainServer.Connect">Connect</see> method.
        /// </summary>
        Connected,

        /// <summary>
        /// Not yet connected to any domain server.
        /// </summary>
        Disconnected,

        /// <summary>
        /// Connection or initialization error.
        /// TODO: Finer grained errors
        /// </summary>
        Error,

        /// <summary>
        /// Native API reported connection status that this implementation does not recognize.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Initializes the client and provides an API for domain server connection.
    /// </summary>
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

        /// <summary>
        /// Initializes the client and provides an API for domain server connection.
        /// </summary>
        /// <param name="ports"> Optional fixed ports to use. </param>
        /// <param name="platformInfo"> Optional platform infomration. TODO: clarify the format. </param>
        /// <param name="userAgent"> User agent string to use when connecting to Metaverse server (default: platform specific). </param>
        /// <param name="appName"> Client application name, used for settings and log files (default: VircadiaClient). </param>
        /// <param name="appOrganization"> Client application organization name, used for settings and log file directory (default: Vircadia). </param>
        /// <param name="appDomain"> Client application domain name. (default: vircadia.com). TODO: figure out what this is for. </param>
        /// <param name="appVersion"> Client application version. (default: native API version). </param>
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

        /// <summary>
        /// Connect or jump to specified location.
        /// </summary>
        /// <param name="location">The address to go to: a "hifi://" address, an IP address (e.g., "127.0.0.1" or "localhost"), a file:/// address, a domain name, a named path on a domain (starts with "/"), a position or position and orientation, or a user (starts with "@").</param>
        public void Connect(string location)
        {
            IntPtr locationPtr = IntPtr.Zero;
            CreateUnmanaged(ref locationPtr, location);
            VircadiaNative.Client.vircadia_connect(this._context, locationPtr);
            DestroyUnmanaged(locationPtr, location);
        }

        /// <summary>
        /// The current status.
        /// </summary>
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

        /// <summary>
        /// A list of connected nodes (assignment clients).
        /// </summary>
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
