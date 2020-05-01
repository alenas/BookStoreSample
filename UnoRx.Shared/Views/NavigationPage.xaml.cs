using System.Reactive.Disposables;

using ReactiveUI;

using UnoRx.ViewModels;


namespace UnoRx.Views {
    public abstract partial class NavigationPageBase : AppReactivePage<NavigationViewModel> {
    }

    /// <summary>
    /// Navigation bindings
    /// </summary>
    public partial class NavigationPage : NavigationPageBase {
        public NavigationPage() {
            this.InitializeComponent();

            this.WhenActivated(disposableRegistration => {
                // Sign-in and Sign-out button bindings
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsSignIn,
                    view => view.btnSing.Content,
                    x => x ? "Sign in" : "Sign out")
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.ButtonPressed,
                    view => view.btnSing)
                    .DisposeWith(disposableRegistration);

            });
        }
    }
}
