//using Microsoft.WindowsAzure.Storage.Blob;
//using Microsoft.WindowsAzure.Storage.Table;
//using System;
//using System.IO;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Web.Http;
//using Zenbo.BotService.Helpers;
//using Zenbo.BotService.Services;

//namespace ImagesWebApi.Controllers
//{
//    public class ClipsController : ApiController
//    {
//        private readonly CloudBlobClient _blobs;
       


//        private CloudBlobContainer GetContainer()
//        {
//            var c = _blobs.GetContainerReference(@"clips");
//            c.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
//            return c;
//        }

     

      

//        [HttpGet]
//        public async Task<IHttpActionResult> Get(string id)
//        {
//            var blob = GetContainer().GetBlockBlobReference(id);

//            if (!await blob.ExistsAsync())
//            {
//                return NotFound();
//            }

//            var content = new StreamContent(blob.OpenRead());
//            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(blob.Properties.ContentType);
//            return ResponseMessage(new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content });
//        }

       

//        // POST api/values
//        [HttpPost]
//        public async Task<IHttpActionResult> Post()
//        {
//            Exception failureException = null;

//            try
//            {
//                var newGuid = Guid.NewGuid().ToString(@"N");

//                using (var ms = new MemoryStream())
//                {
//                    await Request.Content.CopyToAsync(ms);

//                    if (ms.Length > 0)
//                    {
//                        ms.Seek(0, SeekOrigin.Begin);

//                        var blob = GetContainer().GetBlockBlobReference(newGuid);
//                        blob.Properties.ContentType = Request.Content.Headers.ContentType.ToString();
//                        await blob.UploadFromStreamAsync(ms);

//                        var clipBytes = ms.ToArray();
                       

//                        return ResponseMessage(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
//                        {
//                            Content = new StringContent(newGuid)
//                        });
//                    }
//                }
//            }

//            catch (Exception e)
//            {
//                failureException = e;
//            }

//            if (failureException != null)
//            {
//                return BadRequest(failureException.ToString());
//            }
//            else
//            {
//                return BadRequest();
//            }
//        }

         
//        [HttpPut]
//        [Route("{id}")]
//        public async Task<IHttpActionResult> Put(string id)
//        {
//            var blobRef = GetContainer().GetBlockBlobReference(id);
//            if (!await blobRef.ExistsAsync())
//            {
//                return NotFound();
//            }

//            await blobRef.UploadFromStreamAsync(await this.Request.Content.ReadAsStreamAsync());

//            return Ok();
//        }

//        // DELETE api/values/5
//        [HttpDelete]
//        [Route("{id}")]
//        public async Task<IHttpActionResult> Delete(string id)
//        {
//            if (!await GetContainer().GetBlockBlobReference(id).DeleteIfExistsAsync())
//            {
//                return NotFound();
//            }

//            return Ok();
//        }
//    }
//}
