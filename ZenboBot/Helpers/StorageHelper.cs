using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Zenbo.BotService.Contracts;

namespace Zenbo.BotService.Helpers
{
    internal static class StorageHelper
    {
        public static string GetUrlForImage(string imageId) => $"{System.Configuration.ConfigurationManager.AppSettings[@"blobUrl"]}images/{imageId}";
    }
}