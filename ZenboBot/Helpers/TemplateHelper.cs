using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Zenbo.BotService.Helpers
{
    //When building an answer to Zenbo, sometimes we want to show HTML on screen.
    //Here for the sake of simplicity we just take a HTML template and replace the tags with the specifics of what we want
    public class TemplateHelper
    {
        private static DateTime? lastRead = null;
        private static string template = "";

        public static async Task<string> GetTemplate()
        {
            if (!lastRead.HasValue || template=="" || DateTime.Now.Subtract(lastRead.Value).Seconds > 100)
            {
                string templateURL = "https://zenboimages.blob.core.windows.net/fixedstore/template2.html";
                HttpClient client = new HttpClient();
                template = await client.GetStringAsync(templateURL);
                client.Dispose();
            }
            return template;
        }

        public static async Task<string> Transform(string image, string name, string text, bool displayTwitter, bool displayFacebook, bool displayInstagram, bool displayMore)
        {
            string html = await GetTemplate();

            html = html.Replace("{image}", image);
            html = html.Replace("{text}", System.Web.HttpUtility.HtmlEncode(text));
            html = html.Replace("{name}", name);
            html = html.Replace("{displayFacebook}", displayFacebook? "<a href=\"#\" class=\"info\"><img src=\"https://zenboimages.blob.core.windows.net/fixedstore/facebook.png\"></a>" : "");
            html = html.Replace("{displayTwitter}", displayTwitter ? "<a href=\"#\" class=\"info\"><img src=\"https://zenboimages.blob.core.windows.net/fixedstore/twitter.png\"></a>" : "");
            html = html.Replace("{displayInstagram}", displayInstagram ? "<a href=\"#\" class=\"info\"><img src=\"https://zenboimages.blob.core.windows.net/fixedstore/instagram.png\"></a>" : "");
            html = html.Replace("{displayMore}", displayMore? "<a href=\"#\" class=\"info\"><img src=\"https://zenboimages.blob.core.windows.net/fixedstore/more.png\"></a>" : "");


            return html;
        }
    }
}