using System;
using System.Collections.Generic;

namespace Data {

	public class Subscription {
		/// <summary>
		/// Email as User ID
		/// </summary>
		public string id { get; private set; }

		/// <summary>
		/// User's subscribed book ids
		/// </summary>
		public List<Guid> SubscribedBookIds { get; private set; }

		public Subscription(string id, IEnumerable<Guid> subscribedBookIds) {
			this.id = id;
			SubscribedBookIds = new List<Guid>(subscribedBookIds);
		}

		/// <summary>
		/// Adds book Id to subscription
		/// </summary>
		public bool Add(Guid bookId) {
			if (SubscribedBookIds.Contains(bookId)) {
				return false;
			} else {
				SubscribedBookIds.Add(bookId);
				return true;
			}
		}

		/// <summary>
		/// Removes book Id from subscription
		/// </summary>
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
