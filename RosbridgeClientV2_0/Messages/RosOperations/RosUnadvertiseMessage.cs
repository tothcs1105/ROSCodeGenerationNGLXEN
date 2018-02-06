﻿namespace RosbridgeClientV2_0.Messages.RosOperations
{
    using RosbridgeClientV2_0.Constants;

    /// <summary>
    /// Use this class to stop advertising that you are publishing to a topic.
    /// </summary>
    public class RosUnadvertiseMessage : RosTopicMessageBase
    {
        public RosUnadvertiseMessage() : base(RosbridgeProtocolConstants.RosMessages.UNADVERTISE)
        {
        }
    }
}