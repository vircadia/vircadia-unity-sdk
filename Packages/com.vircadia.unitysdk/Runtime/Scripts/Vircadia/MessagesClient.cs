//
//  MessagesClient.cs
//  Runtime/Scripts/Vircadia
//
//  Created by Nshan G. on 31 Mar 2022.
//  Copyright 2022 Vircadia contributors.
//  Copyright 2022 DigiSomni LLC.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.Runtime.InteropServices;

namespace Vircadia
{

    /// <summary>
    /// Flags representing message types.
    /// </summary>
    [Flags]
    public enum MessageType : byte
    {
        /// <summary>
        /// Text messages.
        /// </summary>
        Text = 1 << 0,

        /// <summary>
        /// Data messages.
        /// </summary>
        Data = 1 << 1,

        /// <summary>
        /// Any type of messages.
        /// </summary>
        Any = Text | Data
    }

    public abstract class Message
    {
        /// <summary>
        /// The channel of the message was sent on.
        /// </summary>
        public string Channel { get; internal set; }

        /// <summary>
        /// Whether the message was sent as local only.
        /// </summary>
        public bool LocalOnly { get; internal set; }

        /// <summary>
        /// A universal unique ID of the message sender. The string
        /// representation is different from the server's, see \ref
        /// ./docs/guid.md "Guid notes".
        /// </summary>
        public Guid Sender { get; internal set; }

        internal abstract void SetNativeMessage(IntPtr text, int size);
    }

    public class TextMessage : Message
    {
        /// <summary>
        /// The body of the message.
        /// </summary>
        public string Text { get; internal set; }

        internal override void SetNativeMessage(IntPtr text, int size)
        {
            Text = Marshal.PtrToStringAnsi(text);
        }
    }

    public class DataMessage : Message
    {
        /// <summary>
        /// The body of the message.
        /// </summary>
        public byte[] Data { get; internal set; }

        internal override void SetNativeMessage(IntPtr data, int size)
        {
            Data = new byte[size];
            Marshal.Copy(data, Data, 0, Data.Length);
        }
    }

    /// <summary>
    /// Event handler type for receiving text messages.
    /// </summary>
    /// <param name="message">The received text messages.</param>
    public delegate void TextMessageHandler(TextMessage message);

    /// <summary>
    /// Event handler type for receiving data messages.
    /// </summary>
    /// <param name="message">The received data messages.</param>
    public delegate void DataMessageHandler(DataMessage message);

    /// <summary>
    /// Provides an API for sending and receiving messages.
    /// </summary>
    public class MessagesClient
    {

        /// <summary>
        /// Fires whenever a text message is received.
        /// Text message type needs to be enabled with <see
        /// cref="Vircadia.MessagesClient.Enable"> Enable </see> method, a
        /// specific channel needs to be subscribed to with <see
        /// cref="Vircadia.MessagesClient.Subscribe"> Subscribe </see> method,
        /// and <see cref="Vircadia.MessagesClient.Update"> Update </see>
        /// method needs to be called for these event to fire.
        /// </summary>
        public event TextMessageHandler TextMessageReceived = delegate {};

        /// <summary>
        /// Fires whenever a data message is received.
        /// Text message type needs to be enabled with <see
        /// cref="Vircadia.MessagesClient.Enable"> Enable </see> method, a
        /// specific channel needs to be subscribed to with <see
        /// cref="Vircadia.MessagesClient.Subscribe"> Subscribe </see> method,
        /// and <see cref="Vircadia.MessagesClient.Update"> Update </see>
        /// method needs to be called for these event to fire.
        /// </summary>
        public event DataMessageHandler DataMessageReceived = delegate {};

        /// <summary>
        /// Enables handling of specified message types. This will cause
        /// incoming messages (on channels this client <see
        /// cref="Vircadia.MessagesClient.Subscribe"> subscribed </see> to) to
        /// be buffered. The <see cref="Vircadia.MessagesClient.Update"> Update
        /// </see> method must be called periodically afterwards to clear the
        /// internal buffers and deliver events.
        /// </summary>
        /// <param name="types">Messages types to enable handling for.</param>
        public void Enable(MessageType types = MessageType.Any)
        {
            if(VircadiaNative.Messages.vircadia_enable_messages(_context, (byte)types) == 0)
            {
                _enabledTypes |= types;
            }
        }

        /// <summary>
        /// Subscribe to receive messages on the specified channel.
        /// Message are sent on specified channels, and subscription is
        /// necessary to receive messages from a given channel.
        /// </summary>
        /// <param name="channel">The channel to subscribe to.</param>
        public void Subscribe(string channel)
        {
            IntPtr channelPtr = IntPtr.Zero;
            Utils.CreateUnmanaged(ref channelPtr, channel);
            VircadiaNative.Messages.vircadia_messages_subscribe(_context, channelPtr);
            Utils.DestroyUnmanaged(channelPtr, channel);
        }

        /// <summary>
        /// Unsubscribe from receiving messages on the specified
        /// channel.  Message are always sent on specific channels, and
        /// subscription is necessary to receive messages from a given
        /// channel.
        /// </summary>
        /// <param name="channel">The channel to unsubscribe
        /// from.</param>
        public void Unsubscribe(string channel)
        {
            IntPtr channelPtr = IntPtr.Zero;
            Utils.CreateUnmanaged(ref channelPtr, channel);
            VircadiaNative.Messages.vircadia_messages_unsubscribe(_context, channelPtr);
            Utils.DestroyUnmanaged(channelPtr, channel);
        }

        /// <summary>
        /// The currently buffered text messages. These are updated
        /// whenever <see cref="Vircadia.MessagesClient.Update"> Update
        /// </see> method is called. To not miss any messages, Update
        /// should be called only only after handling all the current
        /// messages. Alternatively the <see
        /// cref="Vircadia.MessagesClient.TextMessageReceived">
        /// TextMessageReceived </see> event can be used.
        /// </summary>
        public TextMessage[] TextMessages
        {
            get; private set;
        }

        /// <summary>
        /// The currently buffered data messages. These are updated
        /// whenever <see cref="Vircadia.MessagesClient.Update"> Update
        /// </see> method is called. To not miss any messages, Update
        /// should be called only only after handling all the current
        /// messages. Alternatively the <see
        /// cref="Vircadia.MessagesClient.DataMessageReceived">
        /// DataMessageReceived </see> event can be used.
        /// </summary>
        public DataMessage[] DataMessages
        {
            get; private set;
        }

        /// <summary>
        /// Updates the message buffers and sends out events for message types
        /// that have been enabled with <see
        /// cref="Vircadia.MessagesClient.Enable"> Enable </see> method, and
        /// channels that have been subscribed to with <see
        /// cref="Vircadia.MessagesClient.Subscribe"> Subscribe </see> methos.
        /// </summary>
        public void Update()
        {
            VircadiaNative.Messages.vircadia_clear_messages(_context, (byte)_enabledTypes);
            VircadiaNative.Messages.vircadia_update_messages(_context, (byte)_enabledTypes);

            TextMessages = GetMessages<TextMessage>(MessageType.Text);
            DataMessages = GetMessages<DataMessage>(MessageType.Data);

            foreach (var message in TextMessages ?? new TextMessage[0])
            {
                TextMessageReceived(message);
            }

            foreach (var message in DataMessages ?? new DataMessage[0])
            {
                DataMessageReceived(message);
            }
        }

        /// <summary>
        /// Sends a text message on specified channel.
        /// </summary>
        /// <param name="channel">The channel to send the message on. </param>
        /// <param name="text">The main body of the message. </param>
        /// <param name="localOnly">Whether to send only locally.</param>
        public void SendTextMessage(string channel, string text, bool localOnly = false)
        {
            IntPtr channelPtr = IntPtr.Zero;
            Utils.CreateUnmanaged(ref channelPtr, channel);
            IntPtr textPtr = IntPtr.Zero;
            Utils.CreateUnmanaged(ref textPtr, text);

            VircadiaNative.Messages.vircadia_send_message(_context, (byte)MessageType.Text,
                channelPtr, textPtr, -1, localOnly ? (byte)1 : (byte)0);

            Utils.DestroyUnmanaged(channelPtr, channel);
            Utils.DestroyUnmanaged(textPtr, text);
        }

        /// <summary>
        /// Sends a data message on specified channel.
        /// </summary>
        /// <param name="channel">The channel to send the message on. </param>
        /// <param name="data">The main body of the message. </param>
        /// <param name="localOnly">Whether to send only locally.</param>
        public void SendDataMessage(string channel, byte[] data, bool localOnly = false)
        {
            IntPtr channelPtr = IntPtr.Zero;
            Utils.CreateUnmanaged(ref channelPtr, channel);

            unsafe
            {
                fixed (byte* dataPtr = data)
                {
                    VircadiaNative.Messages.vircadia_send_message(_context, (byte)MessageType.Data,
                        channelPtr, (IntPtr)dataPtr, data.Length, (byte)(localOnly ? 1 : 0));
                }
            }

            Utils.DestroyUnmanaged(channelPtr, channel);
        }

        private int _context;
        private MessageType _enabledTypes;

        internal MessagesClient(DomainServer domainServer)
        {
            _context = domainServer.ContextId;
            _enabledTypes = 0;
        }

        private MessageClass[] GetMessages<MessageClass>(MessageType type)
            where MessageClass : Message, new()
        {
            byte nativeType = (byte)type;
            int count = VircadiaNative.Messages.vircadia_messages_count(_context, nativeType);
            if (count < 0)
            {
                return null;
            }

            MessageClass[] messages = new MessageClass[count];
            for (int i = 0; i < count; ++i)
            {
                IntPtr message = VircadiaNative.Messages.vircadia_get_message(_context, nativeType, i);
                int size = VircadiaNative.Messages.vircadia_get_message_size(_context, nativeType, i);
                string channel = Marshal.PtrToStringAnsi(
                    VircadiaNative.Messages.vircadia_get_message_channel(_context, nativeType, i));
                Guid? sender = Utils.getUUID(
                    VircadiaNative.Messages.vircadia_get_message_sender(_context, nativeType, i));
                int local = VircadiaNative.Messages.vircadia_is_message_local_only(_context, nativeType, i);
                if (message == IntPtr.Zero || size < 0 || channel == null || sender == null || local < 0)
                {
                    return null;
                }
                else
                {
                    messages[i] = new MessageClass();
                    messages[i].Channel = channel;
                    messages[i].Sender = sender.Value;
                    messages[i].LocalOnly = local == 1;
                    messages[i].SetNativeMessage(message, size);
                }
            }

            return messages;
        }

        ~MessagesClient()
        {
            VircadiaNative.Messages.vircadia_update_messages(_context, (byte)_enabledTypes);
            VircadiaNative.Messages.vircadia_clear_messages(_context, (byte)_enabledTypes);
        }

    }
}
