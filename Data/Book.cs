using System;

namespace Data {

    public class Book {
        public string ASIN { get; set; }
        public string IMAGE_URL { get; set; }
        public string TITLE { get; set; }
        public string AUTHOR { get; set; }
        public int CATEGORY_ID { get; set; }
        public string CATEGORY { get; set; }
        public Guid ID { get; set; }
        public double PRICE { get; set; }
    }
}
