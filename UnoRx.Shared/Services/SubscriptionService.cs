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

    /// <summary>
    /// Subscriptions Service
    /// </summary>
    public class SubscriptionService : ReactiveObject, IEnableLogger {

        /// <summary>
        /// Authentication service
        /// </summary>
        B2CAuthenticationService _authService;

        /// <summary>
        /// User Context
        /// </summary>
        UserContext _user;

        /// <summary>
        /// Users Subscription
        /// </summary>
        [Reactive]
        public Subscription Subscription { get; private set; }

        /// <summary>
        /// Subscription Service depends on Authentication Client
        /// </summary>
        /// <param name="auth">B2CAuthenticationService</param>
        public SubscriptionService(B2CAuthenticationService auth) {
            _authService = auth;
        }

        /// <summary>
        /// Loads users subscriptions from API
        /// </summary>
        public async Task Load() {
            if (!await AuthenticateUser())
                return;
            try {
                Subscription = await new apiClient(new System.Net.Http.HttpClient()).GetSubscriptionsAsync(_user.AccessToken);
                if (Subscription == null)
                    Subscription = new Subscription() { SubscribedBookIds = new List<Guid>() };
            } catch (Exception ex) {
                this.Log().Error(ex, "load subscriptions failed");
                throw ex;
            }
        }

        /// <summary>
        /// Adds book to users subscription
        /// </summary>
        /// <param name="bookId">Book ID</param>
        public async Task Add(Guid bookId) {
            if (!await AuthenticateUser())
                return;
            try {
                Subscription = await new apiClient(new System.Net.Http.HttpClient()).SubscribeAsync(_user.AccessToken, bookId);
            } catch (Exception ex) {
                this.Log().Error(ex, "add subscription failed");
                throw ex;
            }
        }

        /// <summary>
        /// Removes book from users subscription
        /// </summary>
        /// <param name="bookId">Book ID</param>
        public async Task Remove(Guid bookId) {
            if (!await AuthenticateUser())
                return;
            try {
                Subscription = await new apiClient(new System.Net.Http.HttpClient()).UnsubscribeAsync(_user.AccessToken, bookId);
            } catch (Exception ex) {
                this.Log().Error(ex, "remove subscriptions failed");
                throw ex;
            }
        }

        /// <summary>
        /// Signs Out
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
