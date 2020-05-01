using System.Reactive;
using System.Reactive.Linq;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StoreAPI;

using UnoRx.Services;

namespace UnoRx.ViewModels {

	public class BookViewModel : ReactiveObject {

		[Reactive]
		public Book Book { get; set; }

		[Reactive]
		public bool IsSubscribed { get; private set; }

		[Reactive]
		public bool ButtonsVisible { get; private set; }


		public ReactiveCommand<Unit, Unit> Subscribe { get; }
		public ReactiveCommand<Unit, Unit> UnSubscribe { get; }

		public BookViewModel(Book book, SubscriptionService subs) {
			Book = book;
			ButtonsVisible = false;

			subs
				.WhenAnyValue(x => x.Subscription)
				.Select(s => s == null)
				.ToProperty(this, x => x.ButtonsVisible);

			// update subscription status by observing users subscription
			subs
				.WhenAnyValue(x => x.Subscription)
				.Where(s => s != null && s.SubscribedBookIds != null)
				.Select(s => s.SubscribedBookIds.Contains(this.Book.Id))
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(Observer.Create<bool>(SetSubscription));

			Subscribe = ReactiveCommand.CreateFromTask(async () => {
				await subs.Add(Book.Id);
			});

			UnSubscribe = ReactiveCommand.CreateFromTask(async () => {
				await subs.Remove(Book.Id);
			});
		}

		public void SetSubscription(bool value) {
			ButtonsVisible = true;
			IsSubscribed = value;
		}
	}
}
