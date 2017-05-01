using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Zenbo.BotService.Contracts;
using static Zenbo.BotService.Contracts.RobotFeedback;

namespace Zenbo.BotService.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<IMessageActivity>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task DialogCallBackAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                var message = await result;
                
                //Robot sends a first message to notify our service it's up and running. Right now we don't do anything with it, just ignore it
                if (message.Text.ToLower() == "app started")
                {
                    context.Wait(this.MessageReceivedAsync);
                    return;
                }


                if (!string.IsNullOrEmpty(message.Text))
                {
                    //We run the user query agianst LUIS
                    LuisService luis = new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings[@"LUISAppID"], ConfigurationManager.AppSettings[@"LUISKey"]));
                    var luisResult = await luis.QueryAsync(message.Text, System.Threading.CancellationToken.None);

                    //For the sake of simplicity we consider 3 scenarios: 
                    //"Look" - a user is asking the robot to coment about something it sees
                    //"Query name" a user is asking information about a given topic
                    //default: Everything else we can't understand, we just run against the knowledge graph (potentially we could use Bing Search here as well)
                    switch (luisResult.TopScoringIntent.Intent)
                    {
                        case "Look":
                            await context.Forward(new DrawingDialog(), this.DialogCallBackAsync, message, System.Threading.CancellationToken.None);
                            break;
                        case "Query Name":
                            //The trick here is to simplify the work for the knowledge graph: Let's say the user answers "this is a picture of Isaac Newtson",
                            //In this case we extract only the entity "Name" (see our LUIS model in this repo) and only send that forward to Bing. So Bing doesn't need to
                            //understand the context of the conversation, but just find the relevant information about Isaac newton for us
                            if (luisResult.Entities.Any(e => e.Type == "Name"))
                                message.Text = (from e in luisResult.Entities
                                                where e.Type == "Name"
                                                select e.Entity).First();
                            await context.Forward(new KnowledgeGraphDialog(), this.DialogCallBackAsync, message, System.Threading.CancellationToken.None);
                            
                            break;
                        default:
                            await context.Forward(new KnowledgeGraphDialog(), this.DialogCallBackAsync, message, System.Threading.CancellationToken.None);
                            
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());

                await context.PostAsync($"Sorry, I don't think I understood. Can you try again?");
                context.Wait(this.MessageReceivedAsync);
            }
        }

       
    }
}