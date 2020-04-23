using System.Collections.Generic;
using System.Linq;

using Data;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace StoreAPI {

    public static class Books {

        [FunctionName("Books")]
        public static IEnumerable<Book> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                [CosmosDB("BookStoreDB", "BookStore", ConnectionStringSetting = "DBConnection")] IEnumerable<Book> books,
                ILogger log) {

            if (books != null) {
                log.LogInformation($"Book count: {books.Count()}");
            } else {
                log.LogError("There are no books in BookStore collection");
            }

            return books;
        }
    }
}
