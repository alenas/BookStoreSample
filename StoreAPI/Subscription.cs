using System;
using System.Collections.Generic;

namespace StoreAPI {

	/// <summary>
	/// Users subscirption data
	/// </summary>
	public class Subscription {

		/// <summary>
		/// Email as User ID
		/// </summary>
		public string id { get; }

		/// <summary>
		/// List of Users subscribed book ids
		/// </summary>
		public List<Guid> SubscribedBookIds { get; }

		/// <summary>
		/// Default constructor used for JSON serialization
		/// </summary>
		/// <param name="id">Email</param>
		/// <param name="subscribedBookIds">Book Id</param>
		public Subscription(string id, List<Guid> subscribedBookIds) {
			this.id = id;
			SubscribedBookIds = subscribedBookIds;
		}

		/// <summary>
		/// Constructor with a single Book Id
		/// </summary>
		/// <param name="id">Email</param>
		/// <param name="bookId">Book Id</param>
		public Subscription(string id, Guid bookId) {
			this.id = id;
			SubscribedBookIds = new List<Guid>();
			SubscribedBookIds.Add(bookId);
		}

		/// <summary>
		/// Adds Book Id to subscription
		/// </summary>
		/// <returns>false if already subsribed</returns>
		public bool Add(Guid bookId) {
			if (SubscribedBookIds.Contains(bookId)) {
				return false;
			} else {
				SubscribedBookIds.Add(bookId);
				return true;
			}
		}

		/// <summary>
		/// Removes Book Id from subscription
		/// </summary>
		/// <returns>false if book is not subscribed</returns>
		public bool Remove(Guid bookId) {
			if (SubscribedBookIds.Contains(bookId)) {
				SubscribedBookIds.Remove(bookId);
				return true;
			} else {
				return false;
			}

		}
	}
}
