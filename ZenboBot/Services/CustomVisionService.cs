using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Zenbo.BotService.Services
{
    //This wraps our calls to the Custom Vision API. It's basically a REST caller that sends a parameter with our image URL to be analyzed 
    public class CustomVisionService : BaseService
    {
        private readonly string _key;
        private readonly string _model;
        private readonly string _iterationId;

        public CustomVisionService(string predictionKey, string predictionModel, string iterationId)
        {
            _key = predictionKey;
            _model = predictionModel;
            _iterationId = iterationId;
        }

        private HttpClient GetWebClient(bool forImage = false)
        {
            
            HttpClient retVal;
            if (forImage)
            {
                retVal = new HttpClient { BaseAddress = new Uri($@"{ConfigurationManager.AppSettings["CustomVisionURL"]}{_model}/image?iterationId={_iterationId}") };
            }
            else
            {
                retVal = new HttpClient { BaseAddress = new Uri($@"{ConfigurationManager.AppSettings["CustomVisionURL"]}{_model}/url?iterationId={_iterationId}") };
            }

            retVal.DefaultRequestHeaders.Add(@"Prediction-Key", _key);
            return retVal;
        }

        public async Task<string[]> AnalyzeAsync(Stream imageStream)
        {
            using (var client = GetWebClient(true))
            using (var content = new StreamContent(imageStream))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(@"application/octet-stream");

                var response = await client.PostAsync(string.Empty, content);

                return await ProcessResponse(response);
            }
        }

        public async Task<string[]> AnalyzeAsync(string imageUri)
        {
            try
            {
                using (var client = GetWebClient())
                using (var content = new StringContent($@"{{ ""url"" : ""{imageUri}"" }}"))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(@"application/json");

                    var response = await client.PostAsync(string.Empty, content);

                    return await ProcessResponse(response);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return new string[0];
            }
        }

        new private async Task<string[]> ProcessResponse(HttpResponseMessage response)
        {
            List<string> tags = new List<string>();
            if (response.IsSuccessStatusCode)
            {
                JObject result = JObject.Parse(await response.Content.ReadAsStringAsync());
                foreach (JObject tag in (JArray)result["PredictedTags"])
                {
                    if (tag["Probability"].Value<double>() > double.Parse(ConfigurationManager.AppSettings[@"CustomVisionProbabilityThreshold"]))
                    {
                        tags.Add(tag["Tag"].Value<string>());
                    }
                }
                return tags.ToArray();
            }
            else
            {
                return new string[0];
            }
        }
    }
}