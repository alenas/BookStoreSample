namespace StoreAPI.Authentication {

    /// <summary>
    /// Azure AD B2C tenant configuration
    /// </summary>
    public static class B2CConstants {
        public const string Tenant = "bookstoreorg.onmicrosoft.com";
        public const string AzureADB2CHostname = "bookstoreorg.b2clogin.com";
        public const string ClientID = "2f37d7ea-0f70-4138-a0bf-ae5ce715011a";
        public const string PolicySignUpSignIn = "B2C_1_signin";
        public const string PolicyEditProfile = "B2C_1_edit";
        public const string PolicyResetPassword = "B2C_1_reset";

        public static readonly string[] Scopes = { "https://bookstoreorg.onmicrosoft.com/api/access" };

        public static readonly string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public static readonly string AuthoritySignInSignUp = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static readonly string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static readonly string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";
        //public const string IOSKeyChainGroup = "com.microsoft.adalcache";
    }
}