using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using StoreAPI;

using UnoRx.ViewModels;

namespace UnoRx.Services {

    public class BookService {

        private readonly SubscriptionService _subs;

        public BookService(SubscriptionService subs) {
            _subs = subs;
        }

        /// <summary>
        /// Gets all books from API
        /// </summary>
        /// <returns>list of books</returns>
        public IObservable<IEnumerable<BookViewModel>> Get() {
            return Observable.FromAsync(async () => {
                var books = await new apiClient(new System.Net.Http.HttpClient()).BooksAsync();
                return books.Select(x => new BookViewModel(x, _subs));
            });
        }

    }
}
