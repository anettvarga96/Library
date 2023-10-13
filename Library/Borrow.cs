namespace Library
{
    public class Borrow
    {
        public Book Book{ get; set; }
        public string BorrowerName{ get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public Borrow(Book _book, string _name, DateTime _borrowDate)
        {
            Book = _book;
            BorrowerName = _name;
            BorrowDate = _borrowDate;
            ReturnDate = null;
        }

    }
}
