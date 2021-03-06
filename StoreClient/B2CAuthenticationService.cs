﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Identity.Client;

using Newtonsoft.Json.Linq;

namespace StoreAPI.Authentication {

    /// <summary>
    ///  Azure AD B2C Authentication service 
    /// </summary>
    /// <see cref="https://github.com/Azure-Samples/active-directory-b2c-xamarin-native"/>
    public class B2CAuthenticationService {

        private readonly IPublicClientApplication _pca;

        private static readonly Lazy<B2CAuthenticationService> lazy = new Lazy<B2CAuthenticationService>
           (() => new B2CAuthenticationService());

        public static B2CAuthenticationService Instance { get { return lazy.Value; } }

        /// <summary>
        /// Initializes authentication services
        /// </summary>
        private B2CAuthenticationService() {
            // default redirectURI; each platform specific project will have to override it with its own
            var builder = PublicClientApplicationBuilder.Create(B2CConstants.ClientID)
                .WithB2CAuthority(B2CConstants.AuthoritySignInSignUp)
                //.WithIosKeychainSecurityGroup(B2CConstants.IOSKeyChainGroup)
                .WithRedirectUri($"msal{B2CConstants.ClientID}://auth");

            _pca = builder.Build();
        }

        /// <summary>
        /// Tries silently sign-in user, and if fails then calls interactive sign-in
        /// </summary>
        /// <returns>User Context</returns>
        public async Task<UserContext> SignInAsync() {
            UserContext newContext;
            try {
                // acquire token silent
                newContext = await AcquireTokenSilent();
            } catch (MsalUiRequiredException) {
                // acquire token interactive
                newContext = await SignInInteractively();
            }
            return newContext;
        }

        /// <summary>
        /// Resets user password
        /// </summary>
        /// <returns>User Context</returns>
        public async Task<UserContext> ResetPasswordAsync() {
            AuthenticationResult authResult = await _pca.AcquireTokenInteractive(B2CConstants.Scopes)
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(B2CConstants.AuthorityPasswordReset)
                .ExecuteAsync();

            var userContext = UpdateUserInfo(authResult);

            return userContext;
        }

        /// <summary>
        /// Edits users profile
        /// </summary>
        /// <returns>User Context</returns>
        public async Task<UserContext> EditProfileAsync() {
            IEnumerable<IAccount> accounts = await _pca.GetAccountsAsync();

            AuthenticationResult authResult = await _pca.AcquireTokenInteractive(B2CConstants.Scopes)
                .WithAccount(GetAccountByPolicy(accounts, B2CConstants.PolicyEditProfile))
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(B2CConstants.AuthorityEditProfile)
                .ExecuteAsync();

            var userContext = UpdateUserInfo(authResult);

            return userContext;
        }

        /// <summary>
        /// Sing Out
        /// </summary>
        /// <returns>User Context</returns>
        public async Task<UserContext> SignOutAsync() {
            IEnumerable<IAccount> accounts = await _pca.GetAccountsAsync();
            while (accounts.Any()) {
                await _pca.RemoveAsync(accounts.FirstOrDefault());
                accounts = await _pca.GetAccountsAsync();
            }
            var signedOutContext = new UserContext();
            signedOutContext.IsLoggedOn = false;
            return signedOutContext;
        }

        /// <summary>
        /// Silent authentication if token is already in a cache
        /// </summary>
        /// <returns>User Context</returns>
        private async Task<UserContext> AcquireTokenSilent() {
            IEnumerable<IAccount> accounts = await _pca.GetAccountsAsync();
            AuthenticationResult authResult = await _pca.AcquireTokenSilent(B2CConstants.Scopes, GetAccountByPolicy(accounts, B2CConstants.PolicySignUpSignIn))
               .WithB2CAuthority(B2CConstants.AuthoritySignInSignUp)
               .ExecuteAsync();

            var newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        /// <summary>
        /// Interactive log-on
        /// </summary>
        /// <returns>User Context</returns>
        private async Task<UserContext> SignInInteractively() {
            IEnumerable<IAccount> accounts = await _pca.GetAccountsAsync();

            AuthenticationResult authResult = await _pca.AcquireTokenInteractive(B2CConstants.Scopes)
                .WithAccount(GetAccountByPolicy(accounts, B2CConstants.PolicySignUpSignIn))
                .ExecuteAsync();

            var newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        private IAccount GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy) {
            foreach (var account in accounts) {
                string userIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
                if (userIdentifier.EndsWith(policy.ToLower())) return account;
            }

            return null;
        }

        private string Base64UrlDecode(string s) {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

        UserContext UpdateUserInfo(AuthenticationResult ar) {
            var newContext = new UserContext();
            newContext.IsLoggedOn = false;
            JObject user = ParseIdToken(ar.IdToken);

            newContext.AccessToken = ar.AccessToken;
            newContext.Name = user["name"]?.ToString();
            newContext.UserIdentifier = user["oid"]?.ToString();

            newContext.GivenName = user["given_name"]?.ToString();
            newContext.FamilyName = user["family_name"]?.ToString();

            newContext.StreetAddress = user["streetAddress"]?.ToString();
            newContext.City = user["city"]?.ToString();
            newContext.Province = user["state"]?.ToString();
            newContext.PostalCode = user["postalCode"]?.ToString();
            newContext.Country = user["country"]?.ToString();

            newContext.JobTitle = user["jobTitle"]?.ToString();

            var emails = user["emails"] as JArray;
            if (emails != null) {
                newContext.EmailAddress = emails[0].ToString();
            }
            newContext.IsLoggedOn = true;

            return newContext;
        }

        JObject ParseIdToken(string idToken) {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }
    }
}