﻿namespace RosbridgeClientV2_0.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// Abstract class for classes which have topic property
    /// </summary>
    public abstract class RosTopicMessageBase : RosMessageBase
    {
        /// <summary>
        /// The name of the topic to advertise
        /// </summary>
        [JsonProperty("topic")]
        public string Topic { get; set; }
    }
}