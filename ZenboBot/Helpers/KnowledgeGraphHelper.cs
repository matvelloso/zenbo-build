using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Zenbo.BotService.Helpers;

namespace Zenbo.BotService.Helpers
{

    [Serializable]
    public class QueryResult
    {
        internal void SetText(string display = null, string spoken = null, string ssml = null)
        {
            this.DisplayText = display;
            this.SpokenText = string.IsNullOrWhiteSpace(spoken) ? display : spoken;
            this.SpokenSSML = ssml;
        }

        [JsonIgnore]
        internal string DisplayText { get; private set; }
        [JsonIgnore]
        internal string SpokenText { get; private set; }
        [JsonIgnore]
        internal string SpokenSSML { get; private set; }

        public List<ResultEntity> Entities { get; set; }

        public string HTML { get; set; }
        public List<KeyValuePair<string, string>> Cookies { get; set; }

        [JsonIgnore]
        public bool Success { get; set; } = false;
    }

    public class ResultEntity
    {
        public string CurrentEntity { get; set; }
    }

    //This wraps our calls to Bing Knowledge Graph API
    public static class KnowledgeGraphHelper
    {
        private static readonly string BASE_URI = $@"{ConfigurationManager.AppSettings["BingGraphAPI"]}{System.Configuration.ConfigurationManager.AppSettings[@"knowledgeGraphAppId"]}&q=";
      
        public static async Task<QueryResult> QueryAsync(string query, IEnumerable<KeyValuePair<string, string>> cookies,string currentEntity)
        {
            Uri bingUri = new Uri(ConfigurationManager.AppSettings["BingAPIUrl"]);

            //Currently we keep cookies as our way or persisting state between multiple calls
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true
            };
            CookieCollection cookieCollection = null;
            if (cookies != null)
            {
                cookieCollection = new CookieCollection();
                foreach (var cookie in cookies)
                {
                    cookieCollection.Add(new Cookie(cookie.Key, cookie.Value, "/", ConfigurationManager.AppSettings["BingAPIDomain"]));
                }
            }

            if (cookieCollection?.Count > 0) handler.CookieContainer.Add(cookieCollection);

            string response;
            using (var client = new HttpClient(handler))
            {
                response = await client.GetStringAsync(string.Concat(BASE_URI, HttpUtility.UrlEncode(query)));
            }

            cookieCollection = handler.CookieContainer.GetCookies(bingUri);

            QueryResult queryResult = new QueryResult();
            if (cookieCollection.Count > 0)
            {
                queryResult.Cookies = (from Cookie cookie in cookieCollection
                                       where cookie.Name == "SRCHUSR" || cookie.Name == "SRCHUID" || cookie.Name == "MUID"
                                       select new KeyValuePair<string, string>(cookie.Name, cookie.Value)
                                       ).ToList();
            }

            try
            {
                dynamic jObject = JObject.Parse(response);
                string html = "";

                queryResult.SetText((string)jObject?.entities.conversation.displayText, (string)jObject?.entities.conversation.spokenText ?? queryResult.DisplayText, (string)jObject?.entities.conversation.spokenSSML ?? queryResult.DisplayText);

                queryResult.Entities = new List<ResultEntity>();

                //Not all results return entities. When they don't, we get a simple web search. Right now we don't handle that case but it would be as simple as
                //building a HTML response with the web search results
                //We also don't handle cases where multiple entities are returned. In that case we just pick the first one as the most likely
                if (((JArray)jObject["entities"]["value"]).Count > 0)
                {
                    var entity = new ResultEntity { CurrentEntity = ((JArray)jObject["entities"]["value"])[0].Value<string>("name") };
                    queryResult.Entities.Add(entity);
                    //Our custom template HTML just shows how we can send back HTML with the answer. This can be replaced by anything else you might want to show on the robot side
                    html = await TemplateHelper.Transform(((JArray)jObject["entities"]["value"])[0]["image"].Value<string>("thumbnailUrl") ?? ((JArray)jObject["entities"]["value"])[0]["image"].Value<string>("contentUrl"),
                                                    entity.CurrentEntity, queryResult.SpokenText, false, false, false, true);
                }
                else
                    html = await TemplateHelper.Transform("", "", queryResult.SpokenText, false, false, false, true);

                queryResult.HTML = !string.IsNullOrWhiteSpace(html) ? html : null;
                queryResult.Success = true;
            }
            catch (Exception ex)
            {
                queryResult.SetText(@"Hmm... I'm not sure.", @"Sorry, I couldn't find that answer. May be try a different question?");
            }

            return queryResult;
        }
    }
}