﻿namespace Rosbridge.Client.V2_0.Messages.RosOperations
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Rosbridge.Client.V2_0.Constants;

    public class RosServiceResponseMessage : RosMessageBase
    {
        /// <summary>
        /// The name of the service that was called
        /// </summary>
        [JsonProperty("service")]
        public string Service { get; set; }

        /// <summary>
        /// The return values. If the service had no return values, then this field can be omitted (and will be by the rosbridge server)
        /// </summary>
        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public JArray ValueList { get; set; }

        public RosServiceResponseMessage() : base(RosbridgeProtocolConstants.RosMessages.SERVICE_RESPONSE)
        {
        }
    }
}
