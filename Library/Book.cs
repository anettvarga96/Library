using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Book
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public double Price { get; set; }

        public Book(string _title, string _isbn, double _price)
        {
            Title = _title;
            ISBN = _isbn;
            Price = _price;
        }

    }
}
