using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Zenbo.BotService.Contracts;
using Zenbo.BotService.Helpers;
using static Zenbo.BotService.Contracts.RobotFeedback;

namespace Zenbo.BotService.Dialogs
{
    [Serializable]
    public class DrawingDialog : IDialog<IMessageActivity>
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

                await DoDrawingScenario(context, message);
                context.Done(message);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());

                await context.PostAsync($"Error: {ex.ToString()}");
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task DoDrawingScenario(IDialogContext context, IMessageActivity message)
        {
            var incomingChanData = message.GetChannelData<CustomChannelDataRequest>();
            var cogServicesResult = new CognitiveServicesResult();

            var blobClient = System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(CloudBlobClient)) as CloudBlobClient;
            var imageId = !string.IsNullOrWhiteSpace(incomingChanData?.cognitiveRequest?.ImageId) ? incomingChanData.cognitiveRequest.ImageId : "";

            try
            {
                if (!string.IsNullOrWhiteSpace(imageId))
                {
                    var customVision = new Services.CustomVisionService(System.Configuration.ConfigurationManager.AppSettings[@"CustomVisionPredictionKey"],
                        System.Configuration.ConfigurationManager.AppSettings[@"CustomVisionPredictionModel"],
                        System.Configuration.ConfigurationManager.AppSettings[@"CustomVisionIterationID"]);

                    var customVisionResult = await customVision.AnalyzeAsync(StorageHelper.GetUrlForImage(imageId));

                    //In this scenario, our sample custom vision model was built to detect 3 scenarios: 
                    //1-People showing some sort of drawing to the robot
                    //2-People in front of the robot
                    //3-An empty room with no people 

                    if (customVisionResult.Contains("drawing"))
                    {
                        var response = await DialogHelper.CreateResponse(context, message, new[] { Emotions.Doubting }, "I think I'm seeing a drawing. What is it?");
                        await context.PostAsync(response);
                    }
                    else if (customVisionResult.Contains("People"))
                    {
                        var response = await DialogHelper.CreateResponse(context, message, new[] { Emotions.Doubting }, "Yes, I see you. What are you trying to tell me?");
                        await context.PostAsync(response);
                    }
                    else
                    {
                        var response = await DialogHelper.CreateResponse(context, message, new[] { Emotions.Doubting }, "All I see is am empty room. Where is everyone?");
                        await context.PostAsync(response);
                    }
                }
                else
                {
                    var response = await DialogHelper.CreateResponse(context, message, new[] { Emotions.Doubting }, "Sorry, I can't find an image");
                    await context.PostAsync(response);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
        }

      
    }
}