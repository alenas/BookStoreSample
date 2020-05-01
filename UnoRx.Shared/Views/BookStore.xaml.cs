using System.Reactive.Disposables;

using ReactiveUI;

using UnoRx.ViewModels;

namespace UnoRx.Views {

    public abstract partial class BookStoreBase : ReactiveUI.Uno.ReactiveUserControl<BookStoreViewModel> {
    }

    public partial class BookStore : BookStoreBase {

        /// <summary>
        /// Book Store binding between View and ViewModel
        /// </summary>
        public BookStore() {
            this.InitializeComponent();

            this.WhenActivated(disposableRegistration => {
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.BookResults,
                    view => view.bookList.ItemsSource)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsBusy,
                    view => view.progress.IsActive)
                    .DisposeWith(disposableRegistration);

            });
        }

    }
}
