using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class LibraryHandler
    {
        public Dictionary<Book, int> ExistingBooks { get; private set; }
        public List<Borrow> Borrows { get; private set; }
        public IPenaltyCalculator PenaltyCalculator { get; set; }

        public LibraryHandler(Dictionary<Book, int> _books, IPenaltyCalculator _penaltyCalculator)
        {
            ExistingBooks = _books;
            Borrows = new List<Borrow>();
            PenaltyCalculator = _penaltyCalculator;
        }

        public void AddBookToLibrary(string _title, string _isbn, double _price, int _numberOfCopies = 1)
        {
            var myBook = ExistingBooks.Keys.FirstOrDefault(b => b.ISBN == _isbn);

            if (myBook == null)
                ExistingBooks.Add(new Book(_title, _isbn, _price), _numberOfCopies);

            else
                ExistingBooks[myBook] += _numberOfCopies;

        }

        public IEnumerable<Book> GetListOfExistingBooks()
        {
            if (ExistingBooks.Count == 0)
            {
                throw new Exception("Momentan nu avem nici o carte in biblioteca!");
            }

            return ExistingBooks.Keys;
        }

        public int GetNumberOfAvailableCopies(string _isbn)
        {
            var myBook = ExistingBooks.Keys.FirstOrDefault(b => b.ISBN == _isbn);
            return myBook != null ? ExistingBooks[myBook] : 0;
        }


        public IEnumerable<Borrow> GetAllActiveBorrowsForPerson(string _borrowerName)
        {
            return Borrows.FindAll(b => b.BorrowerName == _borrowerName && b.ReturnDate == null);
        }

        public void BorrowCopy(string _isbn, string _borrowerName, DateTime _borrowDate)
        {
            var myBook = ExistingBooks.Keys.FirstOrDefault(b => b.ISBN == _isbn);

            if (myBook != null && ExistingBooks[myBook] > 0)
            {
                Borrows.Add(new Borrow(myBook, _borrowerName, _borrowDate));
                ExistingBooks[myBook]--;
            }

            else
            {
                throw new Exception("Ne pare rau, cartea solicitata nu se afla in biblioteca noastra.");
            }

        }

        public double ReturnCopy(string _isbn)
        {
            var myBook = ExistingBooks.Keys.FirstOrDefault(b => b.ISBN == _isbn);

            if (myBook == null)
            {
                throw new Exception("Aceasta carte nu este proprietatea bibliotecii noastre!");

            }

            var myBorrow = Borrows.Find(b => b.Book == myBook && b.ReturnDate == null);

            if (myBorrow == null)
            {
                throw new Exception("Aceasta carte nu a fost imprumutata de la noi!");

            }

            myBorrow.ReturnDate = DateTime.Today;
            ExistingBooks[myBook]++;

            double penalty = PenaltyCalculator.CalculatePenalty(myBorrow.BorrowDate, myBorrow.ReturnDate.Value, myBorrow.Book.Price);

            if (penalty > 0)
            {
                Console.WriteLine("Termenul de returnare de 14 zile a fost depasit cu {0} zile! Se va aplica o penalizare de {1} RON"
                    , (myBorrow.ReturnDate - myBorrow.BorrowDate).Value.Days
                    , penalty);

            }
            return myBook.Price + penalty;

        }
    }
}
