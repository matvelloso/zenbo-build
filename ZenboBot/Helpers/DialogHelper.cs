using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Zenbo.BotService.Contracts;

namespace Zenbo.BotService.Helpers
{
    public class DialogHelper
    {

        //The custom app running on the robot espects a specific kind of payload which includes not only what the robot is supposed to say, but 
        //also facial expressions, facial movements, etc. This method is just a shortcut to build that payload before answering to the robot
        public static async Task<IMessageActivity> CreateResponse(IDialogContext context, IMessageActivity message, string[] emotions, string text)
        {
            var response = context.MakeMessage();
            response.Text = text;

            var chanData = new CustomChannelDataResponse
            {
                RobotFeedback = new RobotFeedback
                {
                    Emotion = emotions,
                    SpokenText = text
                },
            };

            response.ChannelData = chanData;
            return response;
        }
    }
}