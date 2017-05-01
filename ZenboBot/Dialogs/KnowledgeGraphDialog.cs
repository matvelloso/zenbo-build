using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Zenbo.BotService.Contracts;
using Zenbo.BotService.Helpers;
using static Zenbo.BotService.Contracts.RobotFeedback;

namespace Zenbo.BotService.Dialogs
{
    //This dialog handles the messages that deal with grabbing more information from Bing Knowledge Graph
    [Serializable]
    public class KnowledgeGraphDialog : IDialog<IMessageActivity>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                var message = await result;

                await DoKnowledgeGraphScenario(context, message);

                context.Done(message);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());

                await context.PostAsync($"I don't think I know the answer to that question.");
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private static async Task DoKnowledgeGraphScenario(IDialogContext context, IMessageActivity message)
        {
            var incomingChanData = message.GetChannelData<CustomChannelDataRequest>();

            //We keep cookies between client and Bing, so follow up questions such as "where is he from?" are understood in the context
            //of the previous question
            List<KeyValuePair<string, string>> cookies;
            context.UserData.TryGetValue("Cookies", out cookies);

            //We also keep the entity. We are not doing anything wtih it right now, but it helps any custom logic that needs to know
            //the current topic we're discussing
            string currentEntity = null;
            context.UserData.TryGetValue("Entity", out currentEntity);

            var kbData = await KnowledgeGraphHelper.QueryAsync(message.Text, cookies, currentEntity);
            
            var response = context.MakeMessage();
            response.Text = kbData?.DisplayText;

            var chanData = new CustomChannelDataResponse { Knowledge = kbData };
            chanData.RobotFeedback.SpokenText = kbData?.SpokenText;
            chanData.RobotFeedback.SpokenSSML = kbData?.SpokenSSML;

            if (kbData.Success)
            {
                chanData.RobotFeedback.Emotion = new[] { Emotions.Confident };
            }
            else
            {
                chanData.RobotFeedback.Emotion = new[] { Emotions.Worried, Emotions.Questioning };
            }

            response.ChannelData = chanData;

            if (kbData.Cookies != null)
            {
                context.UserData.SetValue("Cookies", kbData.Cookies);
            }

            var entityToStore = kbData?.Entities?.FirstOrDefault()?.CurrentEntity;
            if (entityToStore != null)
            {
                context.UserData.SetValue("Entity", entityToStore);
            }
            else
            {
                // Otherwise don't change the target entity, user might want to ask a follow up question on same topic
            }

            await context.PostAsync(response);
        }
    }
}