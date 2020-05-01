using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using StoreAPI;
using StoreAPI.Authentication;

namespace UnoRx.Services {

    public class SubscriptionService : ReactiveObject, IEnableLogger {

        B2CAuthenticationService _authService;

        UserContext _user;

        [Reactive]
        public Subscription Subscription { get; private set; }

        //private readonly ObservableAsPropertyHelper<Subscription> _subscription;
        //public Subscription Subscription => _subscription.Value;

        public SubscriptionService(B2CAuthenticationService auth) {
            _authService = auth;
        }

        /// <summary>
        /// Loads users subscriptions from API
        /// </summary>
        public async Task Load() {
            if (!await AuthenticateUser())
                return;

            Subscription = await new apiClient(new System.Net.Http.HttpClient()).GetSubscriptionsAsync(_user.AccessToken);
            if (Subscription == null)
                Subscription = new Subscription() { SubscribedBookIds = new List<Guid>() };
        }

        /// <summary>
        /// Adds book to users subscription
        /// </summary>
        /// <param name="bookId">Book ID</param>
        public async Task Add(Guid bookId) {
            if (!await AuthenticateUser())
                return;

            Subscription = await new apiClient(new System.Net.Http.HttpClient()).SubscribeAsync(_user.AccessToken, bookId);
        }

        /// <summary>
        /// Removes book from users subscription
        /// </summary>
        /// <param name="bookId">Book ID</param>
        public async Task Remove(Guid bookId) {
            if (!await AuthenticateUser())
                return;

            Subscription = await new apiClient(new System.Net.Http.HttpClient()).UnsubscribeAsync(_user.AccessToken, bookId);
        }

        /// <summary>
        /// Logouts
        /// </summary>
        public async Task Logout() {
            await _authService.SignOutAsync();
            Subscription = null;
            _user = null;
        }

        /// <summary>
        /// Logs-in a user
        /// </summary>
        /// <returns>true if user is logged in</returns>
        private async Task<bool> AuthenticateUser() {
            if ((_user == null) || (!_user.IsLoggedOn)) {
                try {
                    _user = await _authService.SignInAsync();
                } catch (Exception ex) {
                    this.Log().Error(ex);
                }
            }
            return (_user != null) && _user.IsLoggedOn;
        }
    }
}
