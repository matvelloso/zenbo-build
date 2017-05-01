using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Zenbo.BotService.Services
{
    public abstract class BaseService
    {
        protected virtual async Task<string> ProcessResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                try
                {
                    string msg = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync()).Value<string>(@"message");

                    throw new Exception(msg);
                }
                catch
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}