﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartQQ.Core.Client;
using SmartQQ.Core.Utils;

namespace SmartQQ.Core.Models
{
    /// <summary>
    ///     好友消息。
    /// </summary>
    public class FriendMessage : IMessage
    {
        [JsonIgnore] internal SmartQQClient Client;

        /// <summary>
        ///     字体。
        /// </summary>
        [JsonIgnore]
        internal Font Font { get; set; }

        /// <summary>
        ///     用于parse消息和字体的对象。
        /// </summary>
        [JsonProperty("content")]
        internal JArray ContentAndFont
        {
            set
            {
                Font = ((JArray) value.First).Last.ToObject<Font>();
                value.RemoveAt(0);
                foreach (var shit in value)
                    Content += StringHelper.ParseEmoticons(shit);
            }
        }

        /// <summary>
        ///     发送者ID。
        /// </summary>
        [JsonProperty("from_uin")]
        internal long SenderId { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public Friend Sender => _sender.GetValue(() => Client.Friends.Find(_ => _.Id == SenderId));

        [JsonIgnore] private readonly LazyHelper<Friend> _sender = new LazyHelper<Friend>();

        [JsonIgnore]
        User IMessage.Sender => Sender;

        /// <inheritdoc />
        [JsonProperty("time")]
        public long Timestamp { get; internal set; }

        /// <inheritdoc />
        [JsonIgnore]
        public string Content { get; internal set; }

        /// <inheritdoc />
        /// <param name="content">回复内容。</param>
        public void Reply(string content)
        {
            Client.Message(SmartQQClient.TargetType.Friend, SenderId, content);
        }

        /// <inheritdoc />
        IMessageable IMessage.RepliableTarget => Sender;
    }
}