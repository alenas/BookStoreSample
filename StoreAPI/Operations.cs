using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using AzureFunctions.Extensions.Swashbuckle.Attribute;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace StoreAPI {

    /// <summary>
    /// Book Store API methods
    /// </summary>
    public static class Operations {

        /// <summary>
        /// Gets a list of all books
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Book[]))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(Error))]
        [FunctionName("Books")]
        public static IEnumerable<Book> GetBooks(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books")] HttpRequest req,
            [SwaggerIgnore]
            [CosmosDB(Constants.DBName, Constants.BookCollection,
                ConnectionStringSetting = Constants.DBConnection)] IEnumerable<Book> books) {
            // just returns books from cosmodb binding
            return books;
        }

        /// <summary>
        /// Gets current users subscriptions
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Subscription))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(Error))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(Error))]
        [RequestHttpHeader("Authorization", isRequired: true)]
        [FunctionName("Subscription")]
        public static async Task<IActionResult> GetSubscription(ILogger log,
            [SwaggerIgnore] ClaimsPrincipal principal,
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "subscription")] HttpRequest req,
            [SwaggerIgnore]
            [CosmosDB(Constants.DBName, Constants.SubscriptionCollection, ConnectionStringSetting = Constants.DBConnection)] DocumentClient client
            ) {

            var email = GetClaimEmail(principal);
            // User not authorized
            if (email == string.Empty) return new UnauthorizedResult();

            Uri documentUri = UriFactory.CreateDocumentUri(Constants.DBName, Constants.SubscriptionCollection, email);
            try {
                var item = await client.ReadDocumentAsync<Subscription>(documentUri,
                    new RequestOptions() { PartitionKey = new PartitionKey(email) });
                return new OkObjectResult(item.Document);
            } catch (DocumentClientException e) when (e.StatusCode == HttpStatusCode.NotFound) {
                return new EmptyResult();
            }
        }

        /// <summary>
        /// Adds a new bookId to users subscription
        /// </summary>
        /// <returns>updated subscription</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Subscription))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(Error))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(Error))]
        [RequestHttpHeader("Authorization", isRequired: true)]
        [FunctionName("Subscribe")]
        public static async Task<IActionResult> Subscribe(ILogger log,
            [SwaggerIgnore] ClaimsPrincipal principal,
            [RequestBodyType(typeof(Guid), "Book ID")]
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "subscription")] HttpRequest req,
            [CosmosDB(Constants.DBName, Constants.SubscriptionCollection, ConnectionStringSetting = Constants.DBConnection)] DocumentClient client
            ) {

            var email = GetClaimEmail(principal);
            // User not authorized
            if (email == string.Empty) return new UnauthorizedResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"Add: {requestBody}");
            Guid bookGuid = ParseGuid(requestBody);

            Uri documentUri = UriFactory.CreateDocumentUri(Constants.DBName, Constants.SubscriptionCollection, email);
            try {
                var item = await client.ReadDocumentAsync<Subscription>(documentUri,
                    new RequestOptions() { PartitionKey = new PartitionKey(email) });
                // add new subscription for the user
                if (!item.Document.Add(bookGuid)) {
                    // already exists
                    return new OkObjectResult(item.Document);
                }
                // Update subscription
                var result = await client.ReplaceDocumentAsync(documentUri, item.Document, new RequestOptions() { PartitionKey = new PartitionKey(email) });

                return new OkObjectResult(result.Resource);
            } catch (DocumentClientException e) when (e.StatusCode == HttpStatusCode.NotFound) {
                // there is no subscription for a user - so create a new one
                var result = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Constants.DBName, Constants.SubscriptionCollection), new Subscription(email, bookGuid), new RequestOptions() { PartitionKey = new PartitionKey(email) });
                return new OkObjectResult(result.Resource);
            }
        }

        /// <summary>
        /// Removes bookId from subscription
        /// </summary>
        /// <returns>updated subscription</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Subscription))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(Error))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(Error))]
        [RequestHttpHeader("Authorization", isRequired: true)]
        [FunctionName("UnSubscribe")]
        public static async Task<IActionResult> UnSubscribe(ILogger log,
            [SwaggerIgnore] ClaimsPrincipal principal,
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "subscription")]
            [RequestBodyType(typeof(Guid), "Book ID")] HttpRequest req,
            [CosmosDB(Constants.DBName, Constants.SubscriptionCollection, ConnectionStringSetting = Constants.DBConnection)] DocumentClient client
            ) {

            var email = GetClaimEmail(principal);
            // User not authorized
            if (email == string.Empty) return new UnauthorizedResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Guid bookGuid = ParseGuid(requestBody);

            Uri documentUri = UriFactory.CreateDocumentUri(Constants.DBName, Constants.SubscriptionCollection, email);
            try {
                var item = await client.ReadDocumentAsync<Subscription>(documentUri,
                    new RequestOptions() { PartitionKey = new PartitionKey(email) });
                // add new subscription for the user
                if (!item.Document.Remove(bookGuid)) {
                    // nothing to unsubscribe from
                    return new OkObjectResult(item.Document);
                }

                var result = await client.ReplaceDocumentAsync(documentUri, item.Document, new RequestOptions() { PartitionKey = new PartitionKey(email) });

                return new OkObjectResult(result.Resource);
            } catch (DocumentClientException e) when (e.StatusCode == HttpStatusCode.NotFound) {
                // there is no document for a user - so skip
                return new OkResult();
            }
        }

        /// <summary>
        /// Parses Guid, as some json serializers put Guids in quotation marks
        /// </summary>
        private static Guid ParseGuid(string guid) {
            guid = guid.Trim();
            if (guid.Length > 36) {
                guid = guid.Substring(1, 36);
            }
            return new Guid(guid);
        }


        /// <summary>
        /// Gets users email from Claim Principal
        /// </summary>
        /// <returns>email or empty string</returns>
        private static string GetClaimEmail(ClaimsPrincipal principal) {
            foreach (var identity in principal?.Identities) {
                // only use Azure AD claims
                if (identity.IsAuthenticated && identity.AuthenticationType == "aad") {
                    var claim = identity.FindFirst("emails");
                    if (claim != null)
                        return claim.Value;
                }
            }
            return string.Empty;
        }

    }
}
