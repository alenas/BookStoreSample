using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Hosting;

using StoreAPI;

[assembly: WebJobsStartup(typeof(SwashBuckleStartup))]
namespace StoreAPI {

    internal class SwashBuckleStartup : IWebJobsStartup {

        public void Configure(IWebJobsBuilder builder) {
            //Register the extension
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly());
        }
    }

    /// <summary>
    /// Swagger endpoints
    /// </summary>
    public static class Swagger {

        /// <summary>
        /// Creates swagger json dosument
        /// </summary>
        [SwaggerIgnore]
        [FunctionName("Swagger")]
        public static Task<HttpResponseMessage> GetJson(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api.json")] HttpRequestMessage req,
            [SwashBuckleClient]ISwashBuckleClient swashBuckleClient) {
            return Task.FromResult(swashBuckleClient.CreateSwaggerDocumentResponse(req));
        }

        /// <summary>
        /// Creates swagger UI
        /// </summary>
        [SwaggerIgnore]
        [FunctionName("SwaggerUi")]
        public static Task<HttpResponseMessage> GetUI(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ui")] HttpRequestMessage req,
            [SwashBuckleClient]ISwashBuckleClient swashBuckleClient) {
            return Task.FromResult(swashBuckleClient.CreateSwaggerUIResponse(req, "api.json"));
        }
    }
}
