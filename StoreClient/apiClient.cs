namespace StoreAPI {

    /// <summary>
    /// Generated client has some funny names.
    /// Normally we would fix a generator class, as this could lead to vulnerabilities (as methods Subscribe and UnSubscribe could get mixed up).
    /// </summary>
    public partial class apiClient {

        /// <summary>Gets current users subscriptions</summary>
        /// <returns>Success and users subscription</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<Subscription> GetSubscriptionsAsync(string token) {
            return Subscription2Async(GetBearer(token), System.Threading.CancellationToken.None);
        }

        /// <summary>Adds a new bookId to users subscription</summary>
        /// <returns>Success and users subscription</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<Subscription> SubscribeAsync(string token, System.Guid? bookId) {
            return SubscriptionAsync(GetBearer(token), bookId, System.Threading.CancellationToken.None);
        }

        /// <summary>Removes bookId from subscription</summary>
        /// <returns>Success and users subscription</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<Subscription> UnsubscribeAsync(string token, System.Guid? bookId) {
            return Subscription3Async(GetBearer(token), bookId, System.Threading.CancellationToken.None);
        }


        private static string GetBearer(string token) {
            return $"Bearer {token}";
        }

    }
}
