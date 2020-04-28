using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Data;

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
        [FunctionName("Get-Books")]
        public static IEnumerable<Book> GetBooks(ILogger log,
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books")] HttpRequest req,
            [CosmosDB(Constants.DBName, Constants.BookCollection,
                ConnectionStringSetting = Constants.DBConnection)] IEnumerable<Book> books) {
            // just returns books from cosmodb binding
            return books;
        }

        /// <summary>
        /// Gets subscribed book ids for a user
        /// </summary>
        [FunctionName("Get-Subscription")]
        public static async Task<IActionResult> GetSubscription(ILogger log, ClaimsPrincipal principal,
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "subscription")] HttpRequest req,
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
        [FunctionName("Subscribe")]
        public static async Task<IActionResult> Subscribe(ILogger log, ClaimsPrincipal principal,
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "subscription")] HttpRequest req,
            [CosmosDB(Constants.DBName, Constants.SubscriptionCollection, ConnectionStringSetting = Constants.DBConnection)] DocumentClient client
            ) {

            var email = GetClaimEmail(principal);
            // User not authorized
            if (email == string.Empty) return new UnauthorizedResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Guid bookGuid = new Guid(requestBody);

            Uri documentUri = UriFactory.CreateDocumentUri(Constants.DBName, Constants.SubscriptionCollection, email);
            try {
                var item = await client.ReadDocumentAsync<Subscription>(documentUri,
                    new RequestOptions() { PartitionKey = new PartitionKey(email) });
                // add new subscription for the user
                if (!item.Document.Add(bookGuid)) {
                    // already exists
                    return new OkObjectResult(item.Document);
                }

                var result = await client.ReplaceDocumentAsync(documentUri, item.Document, new RequestOptions() { PartitionKey = new PartitionKey(email) });

                return new OkObjectResult(result.Resource);
            } catch (DocumentClientException e) when (e.StatusCode == HttpStatusCode.NotFound) {
                // there is no document for a user - so create a new one
                var result = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Constants.DBName, Constants.SubscriptionCollection), new Subscription(email, new Guid[] { bookGuid }), new RequestOptions() { PartitionKey = new PartitionKey(email) });
                return new OkObjectResult(result.Resource);
            }
        }

        /// <summary>
        /// Removes bookId from subscription
        /// </summary>
        /// <returns>updated subscription</returns>
        [FunctionName("UnSubscribe")]
        public static async Task<IActionResult> UnSubscribe(ILogger log, ClaimsPrincipal principal,
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "subscription")] HttpRequest req,
            [CosmosDB(Constants.DBName, Constants.SubscriptionCollection, ConnectionStringSetting = Constants.DBConnection)] DocumentClient client
            ) {

            var email = GetClaimEmail(principal);
            // User not authorized
            if (email == string.Empty) return new UnauthorizedResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Guid bookGuid = new Guid(requestBody);

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
