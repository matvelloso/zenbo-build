using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Zenbo.BotService.Dialogs;

namespace Zenbo.BotService
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            try
            {
                if (activity != null)
                {
                    switch (activity.GetActivityType())
                    {
                        case ActivityTypes.Message:

                            await Conversation.SendAsync(activity, () => new RootDialog());
                            break;

                        case ActivityTypes.ConversationUpdate:
                            break;
                        case ActivityTypes.ContactRelationUpdate:
                        case ActivityTypes.Typing:
                        case ActivityTypes.DeleteUserData:
                        case ActivityTypes.Ping:
                        default:
                            Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }
    }
}
