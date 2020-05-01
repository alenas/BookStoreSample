using ReactiveUI;

namespace UnoRx.ViewModels {

    /// <summary>
    /// All view models should be Ractive
    /// </summary>
    public abstract class ViewModelBase : ReactiveObject {
    }

    /// <summary>
    /// Routable view model
    /// </summary>
    public abstract class RoutableViewModel : ViewModelBase, IRoutableViewModel {
        protected RoutableViewModel(IScreen hostScreen) {
            HostScreen = hostScreen;
        }

        public string UrlPathSegment { get; }
        public IScreen HostScreen { get; }
    }
}
