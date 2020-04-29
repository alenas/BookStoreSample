using System;

namespace StoreAPI {

    /// <summary>
    /// Book
    /// </summary>
    public class Book {

        /// <summary>
        /// ASIN
        /// </summary>
        public string ASIN { get; }

        /// <summary>
        /// URL to books cover image
        /// </summary>
        public string ImageUrl { get; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Author
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Category ID
        /// </summary>
        public int CagegoryId { get; }

        /// <summary>
        /// Category
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Book ID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Price
        /// </summary>
        public double Price { get; }

        /// <summary>
        /// Default constructor for JSON
        /// </summary>
        public Book(string asin, string image_url, string title, string author,
            int category_id, string category, Guid id, double price) {

            ASIN = asin;
            ImageUrl = image_url;
            Title = title;
            Author = author;
            CagegoryId = category_id;
            Category = category;
            Id = id;
            Price = price;
        }
    }
}
