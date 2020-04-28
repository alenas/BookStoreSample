namespace StoreAPI {

	internal class Constants {

		/// <summary>
		/// Azure Cosmos DB DataBase name
		/// </summary>
		internal const string DBName = "BookStoreDB";

		/// <summary>
		/// Key name for a Connection string (either from local.settings.json, or function key)
		/// </summary>
		internal const string DBConnection = "DBConnection";

		/// <summary>
		/// Book collection name
		/// </summary>
		internal const string BookCollection = "BookStore";

		/// <summary>
		/// Subscirption collection name
		/// </summary>
		internal const string SubscriptionCollection = "Subscriptions";
	}
}
