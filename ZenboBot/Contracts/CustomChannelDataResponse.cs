using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zenbo.BotService.Helpers;

namespace Zenbo.BotService.Contracts
{
    public enum ConversationState
    {
        /// <summary> 
        /// This message is something the bot would like to show/do but it's not done talking yet  
        /// and will say something else soon in a separate message, without input from the user. 
        /// </summary> 
        HasMoreToSay,

        /// <summary> 
        /// The bot will wait for a response and take no further action. 
        /// </summary> 
        WaitingForUser,
    }

    [Serializable]
    public class CognitiveServicesResult
    {
        [JsonProperty(@"emotionResult")]
        public JToken EmotionResult { get; set; }
        [JsonProperty(@"visionResult")]
        public JToken VisionResult { get; set; }
        [JsonProperty(@"faceResult")]
        public JToken FaceResult { get; set; }
    }

    [Serializable]
    public class CustomChannelDataResponse
    {

        [JsonProperty(@"knowledge")]
        public QueryResult Knowledge { get; set; }

        [JsonProperty(@"robotFeedback")]
        public RobotFeedback RobotFeedback { get; set; } = new RobotFeedback();

        [JsonProperty("conversationState")]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ConversationState State { get; set; } = ConversationState.WaitingForUser;

        [JsonProperty(@"cognitiveResult")]
        public CognitiveServicesResult CognitiveResult { get; set; }
    }
}