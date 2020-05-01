using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using ReactiveUI;

using UnoRx.Services;

namespace UnoRx.ViewModels {

	public class BookStoreViewModel : RoutableViewModel {

		// Here's the interesting part: In ReactiveUI, we can take IObservables
		// and "pipe" them to a Property - whenever the Observable yields a new
		// value, we will notify ReactiveObject that the property has changed.
		// 
		// To do this, we have a class called ObservableAsPropertyHelper - this
		// class subscribes to an Observable and stores a copy of the latest value.
		// It also runs an action whenever the property changes, usually calling
		// ReactiveObject's RaisePropertyChanged.
		private readonly ObservableAsPropertyHelper<IEnumerable<BookViewModel>> _bookResults;
		public IEnumerable<BookViewModel> BookResults => _bookResults.Value;

		// Here, we want to create a property to represent when the application 
		// is loading books (i.e. when to show the "spinner" control that 
		// lets the user know that the app is busy). We also declare this property
		// to be the result of an Observable (i.e. its value is derived from 
		// some other property)
		private readonly ObservableAsPropertyHelper<bool> _isAvailable;
		public bool IsBusy => !_isAvailable.Value;

		public BookStoreViewModel(IScreen hostScreen, BookService client, SubscriptionService subs) : base(hostScreen) {
			// loads books from BookService and stores in an observable property
			_bookResults = this
				.WhenAnyValue(x => x._bookResults)
				.Where(r => r == null)
				.SelectMany(client.Get())
				.ObserveOn(RxApp.MainThreadScheduler)
				.ToProperty(this, x => x.BookResults);

			// We subscribe to the "ThrownExceptions" property of our OAPH, where ReactiveUI 
			// marshals any exceptions that are thrown in BookService.Get() method. 
			//_bookResults.ThrownExceptions.Subscribe(error => { /* Handle errors here */ });

			// A helper method we can use for Visibility or Spinners to show if results are available.
			_isAvailable = this
				.WhenAnyValue(x => x.BookResults)
				.Select(results => results != null)
				.ToProperty(this, x => x.IsBusy);
		}

	}
}
