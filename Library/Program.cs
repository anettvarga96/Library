using Library;

var book1 = new Book("1984", "9780140817744", 30.15);
var book2 = new Book("The Catcher in the Rye", "9783462015393", 27.00);
var book3 = new Book("To Kill a Mockingbird", "9782253115847", 32.20);

var myLibraryHandler = new LibraryHandler(new Dictionary<Book, int>() {
                                                                        { book1, 1},
                                                                        { book2, 2},
                                                                        { book3, 3}
                                                                        }
                                         , new PenaltyCalculator());

Console.WriteLine("Bine ati venit la biblioteca noastra!");
Console.WriteLine();
Console.WriteLine("Mai jos aveti lista de comenzi disponibile:");
Console.WriteLine();
Console.WriteLine("[C]ere lista tuturor cartilor din biblioteca");
Console.WriteLine("[A]dauga o carte noua");
Console.WriteLine("[N]umarul de exemplare disponibile pentru o anumita carte");
Console.WriteLine("[I]mprumuta o carte");
Console.WriteLine("[R]eturneaza o carte");
Console.WriteLine("[E]xit");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Introduceti o comanda:");
    Console.WriteLine();

    var userInput = Console.ReadLine();

    Console.WriteLine();

    switch (userInput.ToLower())
    {
        case "c":
            var existingBooks = myLibraryHandler.GetListOfExistingBooks();
            foreach (var book in existingBooks)
                Console.WriteLine(book.Title + " | " + book.ISBN + " | " + book.Price);
            break;

        case "a":
            Console.WriteLine("Introduceti titlul cartii:");
            var newTitle = Console.ReadLine();
            Console.WriteLine("Introduceti ISBN-ul:");
            var newIsbn = Console.ReadLine();
            Console.WriteLine("Introduceti pretul cartii:");
            var newPrice = Console.ReadLine();
            Console.WriteLine("Introduceti numarul de exemplare:");
            var newNoOfCopies = Console.ReadLine();

            myLibraryHandler.AddBookToLibrary(newTitle.Trim(), newIsbn.Trim(), Convert.ToDouble(newPrice), Convert.ToInt32(newNoOfCopies));

            break;

        case "n":
            Console.WriteLine("Introduceti ISBN-ul cartii pentru care doriti sa aflati numarul de exemplare disponibile!");
            var searchIsbn = Console.ReadLine();

            Console.WriteLine("Avem {0} exemplar(e) disponibile", myLibraryHandler.GetNumberOfAvailableCopies(searchIsbn.Trim()));
            break;

        case "i":
            Console.WriteLine("Introduceti ISBN-ul cartii pe care doriti sa o imprumutati!");
            var borrowIsbn = Console.ReadLine();
            Console.WriteLine("Introduceti numele dumneavoastra:");
            var borrowerName = Console.ReadLine();
            Console.WriteLine("Introduceti data incepand cu care doriti sa imprumutati!");
            var borrowDate = Console.ReadLine();
            try
            {
                myLibraryHandler.BorrowCopy(borrowIsbn.Trim(), borrowerName.ToLower(), Convert.ToDateTime(borrowDate));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            break;

        case "r":
            Console.WriteLine("Introduceti numele dumneavoastra:");
            var personName = Console.ReadLine();
            var activeBorrows = myLibraryHandler.GetAllActiveBorrowsForPerson(personName.ToLower());

            if (activeBorrows.Count() == 0)
            {
                Console.WriteLine("Nu ati imprumutat nici o carte pana acum!");
                break;
            }
            else
            {
                Console.WriteLine("Mai jos aveti lista cartilor imprumutate de dumneavoastra");
                foreach (var activeBorrow in activeBorrows)
                    Console.WriteLine(activeBorrow.Book.Title + " | " + activeBorrow.Book.ISBN + " | " + activeBorrow.Book.Price);
            }

            Console.WriteLine();
            Console.WriteLine("Introduceti ISBN-ul cartii pe care doriti sa o returnati!");

            var returnIsbn = Console.ReadLine();
            try
            {
                Console.WriteLine("Aveti de platit {0} RON!", myLibraryHandler.ReturnCopy(returnIsbn.Trim()));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            break;

        case "e":
            return;

        default:
            Console.WriteLine("Ne pare rau, aceasta nu este o comanda valida!");
            break;
    }

}

