using System.Reactive.Disposables;

using ReactiveUI;

using UnoRx.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace UnoRx.Views {

    public abstract partial class BookViewBase : ReactiveUI.Uno.ReactiveUserControl<BookViewModel> {
    }

    /// <summary>
    /// Book user control
    /// </summary>
    public partial class BookView : BookViewBase {

        /// <summary>
        /// Book bindings between View and ViewModel
        /// </summary>
        public BookView() {
            this.InitializeComponent();
            this.WhenActivated(disposableRegistration => {
                // we convert from Url into a BitmapImage. 
                // This is an easy way of doing value conversion using ReactiveUI binding.
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Book.ImageUrl,
                    view => view.bookImage.Source,
                    url => url == null ? null : new BitmapImage(new System.Uri(url)))
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Book.Title,
                    view => view.bookTitle.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Book.Price,
                    view => view.bookPrice.Text,
                    price => "$" + price)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsSubscribed,
                    view => view.subButton.Visibility,
                    // inverse visibility converter custom action
                    sub => sub ? Visibility.Collapsed : Visibility.Visible)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.ButtonsVisible,
                    view => view.btnPanel.Visibility)
                    .DisposeWith(disposableRegistration);

                // automatic visibility converter through binding
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsSubscribed,
                    view => view.unButton.Visibility)
                    .DisposeWith(disposableRegistration);

                // commands to Subscribe and UnSubscribe
                this.BindCommand(ViewModel,
                    viewModel => viewModel.Subscribe,
                    view => view.subButton)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.UnSubscribe,
                    view => view.unButton)
                    .DisposeWith(disposableRegistration);


            });
        }
    }
}
