using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Microsoft.Extensions.DependencyInjection;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using UnoRx.Services;

namespace UnoRx.ViewModels {

	[Windows.UI.Xaml.Data.Bindable]
	public class NavigationViewModel : ViewModelBase, IScreen, IActivatableViewModel {

		readonly IServiceProvider _ServiceProvider;

		public NavigationViewModel(IServiceProvider serviceProvider, SubscriptionService subs) {
			_ServiceProvider = serviceProvider;
			SelectedNavigationItem = NavigationItems.First();

			this.WhenActivated(disposables => {
				this
				.WhenAnyValue(vm => vm.SelectedNavigationItem)
				.Select(navItem => navItem.ViewModelType)
				.Select(vmType => (IRoutableViewModel)_ServiceProvider.GetRequiredService(vmType))
				.InvokeCommand(Router.Navigate)
				.DisposeWith(disposables);
			});

			subs
				.WhenAnyValue(x => x.Subscription)
				.Select(s => s != null)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(Observer.Create<bool>(UpdateSubscription));

			ButtonPressed = ReactiveCommand.CreateFromTask(async () => {
				if (IsSignIn) await subs.Load();
				else await subs.Logout();
			});

		}

		public void UpdateSubscription(bool signedIn) {
			IsSignIn = !signedIn;
		}

		public IReadOnlyList<MenuItem> NavigationItems => new List<MenuItem>
		{
			new MenuItem(typeof(BookStoreViewModel), "Book Store", "Home")
		}.AsReadOnly();

		[Reactive]
		public MenuItem SelectedNavigationItem { get; set; }

		[Reactive]
		public bool IsSignIn { get; set; }

		/// <summary>
		/// Sign-in button commands
		/// </summary>
		public ReactiveCommand<Unit, Unit> ButtonPressed { get; }


		public RoutingState Router { get; } = new RoutingState();
		public ViewModelActivator Activator { get; } = new ViewModelActivator();
	}

	[Windows.UI.Xaml.Data.Bindable]
	public class MenuItem {
		public MenuItem(Type viewModelType, string title, string symbol) {
			ViewModelType = viewModelType;
			Title = title;
			Symbol = symbol;
		}

		public Type ViewModelType { get; }

		public string Title { get; }

		public string Symbol { get; }
	}
}
