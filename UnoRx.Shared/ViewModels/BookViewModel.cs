using System.Reactive;
using System.Reactive.Linq;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StoreAPI;

using UnoRx.Services;

namespace UnoRx.ViewModels {

	public class BookViewModel : ReactiveObject {

		/// <summary>
		/// Book
		/// </summary>
		[Reactive]
		public Book Book { get; set; }

		/// <summary>
		/// Is User Subscribed to this book
		/// </summary>
		[Reactive]
		public bool IsSubscribed { get; private set; }

		/// <summary>
		/// Subscribe or unsubscribe button visibility
		/// </summary>
		[Reactive]
		public bool ButtonsVisible { get; private set; }

		// commands that bind to view buttons
		public ReactiveCommand<Unit, Unit> Subscribe { get; }
		public ReactiveCommand<Unit, Unit> UnSubscribe { get; }

		public BookViewModel(Book book, SubscriptionService subs) {
			Book = book;
			ButtonsVisible = false;

			// hides buttons when user is not signed-in
			subs
				.WhenAnyValue(x => x.Subscription)                          // when users subscribption changes
				.Select(s => s != null)                                     // set boolean if not null
				.ObserveOn(RxApp.MainThreadScheduler)                       // observe on UI thread
				.Subscribe(Observer.Create<bool>(SetButtonVisibility));     // observe changes in SetButtonVisibility method

			// update subscription status by observing users subscription
			subs
				.WhenAnyValue(x => x.Subscription)                          // when users subscribption changes
				.Where(s => s != null && s.SubscribedBookIds != null)       // where user has some existing subscribtions
				.Select(s => s.SubscribedBookIds.Contains(this.Book.Id))    // set boolean if subscription contains this book
				.ObserveOn(RxApp.MainThreadScheduler)                       // observe on UI thread
				.Subscribe(Observer.Create<bool>(SetSubscription));         // observe changes in SetSubscription method

			// subscribe command
			Subscribe = ReactiveCommand.CreateFromTask(async () => {
				await subs.Add(Book.Id);
			});

			// unsubscribe command
			UnSubscribe = ReactiveCommand.CreateFromTask(async () => {
				await subs.Remove(Book.Id);
			});
		}

		void SetButtonVisibility(bool value) {
			ButtonsVisible = value;
			// if user logs-out then forget state
			if (!value) IsSubscribed = false;
		}

		void SetSubscription(bool value) {
			IsSubscribed = value;
		}
	}
}
