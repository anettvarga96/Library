using Library;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.UnitTests
{
    [TestFixture]
    public class LibraryHandlerTests
    {
        private LibraryHandler _libraryHandler;
        private Book _book0;
        private Book _book1;
        private Book _book2;
        private Book _book3;
        private Mock<IPenaltyCalculator> _penaltyCalculator;

        private DateTime _today = DateTime.Today;
        private DateTime _yesterday = DateTime.Today.AddDays(-1);
        private DateTime _threeWeeksAgo = DateTime.Today.AddDays(-21);

        private string _borrowerName = "John Doe";

        [SetUp]
        public void Setup()
        {
            _book0 = new Book("title0", "isbn", 10);
            _book1 = new Book("title1", "isbn1", 10.1);
            _book2 = new Book("title2", "isbn2", 10.2);
            _book3 = new Book("title3", "isbn3", 10.3);

            _penaltyCalculator = new Mock<IPenaltyCalculator>();

            _libraryHandler = new LibraryHandler(new Dictionary<Book, int>()
           {
               { _book0, 0 },
               { _book1, 1 },
               { _book2, 2 },
           }, _penaltyCalculator.Object);

        }

        [Test]
        [TestCase(null)]
        [TestCase(2)]
        [TestCase(3)]
        public void AddBookToLibrary_BookAlreadyExistsInLibrary_IncrementsNumberOfAvailableCopies(int numberOfCopies)
        {
            _libraryHandler.AddBookToLibrary(_book1.Title, _book1.ISBN, _book1.Price, numberOfCopies);

            Assert.That(_libraryHandler.ExistingBooks[_book1], Is.EqualTo(1 + numberOfCopies));
        }

        [Test]
        [TestCase(null)]
        [TestCase(1)]
        [TestCase(2)]
        public void AddBookToLibrary_NewBook_AddsBookToExistingBooksAndSetsAvailableCopies(int numberOfCopies)
        {
            _libraryHandler.AddBookToLibrary(_book3.Title, _book3.ISBN, _book3.Price, numberOfCopies);

            Assert.That(_libraryHandler.ExistingBooks.Keys.Any(b=>b.Title == _book3.Title && b.ISBN == _book3.ISBN && b.Price == _book3.Price));
            Assert.That(_libraryHandler.ExistingBooks.LastOrDefault().Value, Is.EqualTo(numberOfCopies));
        }

        [Test]
        public void GetNumberOfAvailableCopies_NoCopiesExist_Returns0()
        {
            var result = _libraryHandler.GetNumberOfAvailableCopies(_book3.ISBN);

            Assert.That(result, Is.Zero);
        }

        [Test]
        public void GetNumberOfAvailableCopies_NoCopiesAvailable_Returns0()
        {

            var result = _libraryHandler.GetNumberOfAvailableCopies(_book0.ISBN);

            Assert.That(result, Is.Zero);
        }

        [Test]
        public void GetNumberOfAvailableCopies_CopiesAvailable_ReturnsNumberOfAvailableCopies()
        {
            var result = _libraryHandler.GetNumberOfAvailableCopies(_book1.ISBN);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void BorrowCopy_NoCopiesAvailable_ThrowsException()
        {
            Assert.That(() => _libraryHandler.BorrowCopy(_book0.ISBN, It.IsAny<string>(), It.IsAny<DateTime>()), Throws.Exception);
        }

        [Test]
        public void BorrowCopy_NoCopiesExist_ThrowsException()
        {
            Assert.That(() => _libraryHandler.BorrowCopy(_book3.ISBN, It.IsAny<string>(), It.IsAny<DateTime>()), Throws.Exception);
        }

        [Test]
        public void BorrowCopy_CopiesExist_NewBorrowObjectIsAdded()
        {
            _libraryHandler.BorrowCopy(_book1.ISBN, _borrowerName, _today);

            Assert.That(_libraryHandler.Borrows.Any(b => b.Book == _book1 && b.BorrowerName == _borrowerName && b.BorrowDate == _today));
            Assert.That(_libraryHandler.ExistingBooks[_book1], Is.Zero);
        }

        [Test]
        public void ReturnCopy_CopyNotOurs_ThrowsException()
        {
            Assert.That(() => _libraryHandler.ReturnCopy(_book3.ISBN), Throws.Exception);
            _penaltyCalculator.Verify(pc => pc.CalculatePenalty(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<double>()), Times.Never);

        }

        [Test]
        public void ReturnCopy_CopyWasNotBorrowed_ThrowsException()
        {
            Assert.That(() => _libraryHandler.ReturnCopy(_book0.ISBN), Throws.Exception);
            _penaltyCalculator.Verify(pc => pc.CalculatePenalty(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<double>()), Times.Never);

        }

        [Test]
        public void ReturnCopy_ReturningInTime_ReturnsBookPrice()
        {
            _libraryHandler.Borrows.Add(new Borrow(_book1, _borrowerName, _yesterday));
            _libraryHandler.ExistingBooks[_book1]--;

            var result = _libraryHandler.ReturnCopy(_book1.ISBN);

            Assert.That(_libraryHandler.Borrows.Any(b => b.Book == _book1 && b.BorrowerName == _borrowerName && b.BorrowDate == _yesterday && b.ReturnDate == _today));
            Assert.That(_libraryHandler.ExistingBooks[_book1], Is.EqualTo(1));
            _penaltyCalculator.Verify(pc => pc.CalculatePenalty(_yesterday, _today, _book1.Price), Times.Once);

            Assert.That(result, Is.EqualTo(_book1.Price));
        }

        [Test]
        public void ReturnCopy_ReturningLate_ReturnsBookPriceWithPenalty()
        {
            _libraryHandler.Borrows.Add(new Borrow(_book1, _borrowerName, _threeWeeksAgo));
            _libraryHandler.ExistingBooks[_book1]--;
            var penalty = _penaltyCalculator.Object.CalculatePenalty(_threeWeeksAgo, _today, _book1.Price);
            var result = _libraryHandler.ReturnCopy(_book1.ISBN);

            Assert.That(_libraryHandler.Borrows.Any(b => b.Book == _book1 && b.BorrowerName == _borrowerName && b.BorrowDate == _threeWeeksAgo && b.ReturnDate == _today));
            Assert.That(_libraryHandler.ExistingBooks[_book1], Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(_book1.Price + penalty));
        }

    }
}
