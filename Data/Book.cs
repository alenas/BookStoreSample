using System;

namespace Data {

    public class Book {

        public string ASIN { get; }
        public string ImageUrl { get; }
        public string Title { get; }
        public string Author { get; }
        public int CagegoryId { get; }
        public string Category { get; }
        public Guid Id { get; }
        public double Price { get; }

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
