using Library;
using NUnit.Framework;
using System;

namespace Library.UnitTests
{
    [TestFixture]
    public class PenaltyCalculatorTests
    {
        private PenaltyCalculator _penaltyCalculator = new PenaltyCalculator();
        private DateTime _today = DateTime.Today;
        private DateTime _yesterday = DateTime.Today.AddDays(-1);
        private DateTime _threeWeeksAgo = DateTime.Today.AddDays(-21);
        double _price = 10;

        [Test]
        public void CalculatePenalty_BorrowDateAfterReturnDate_ThrowsInvalidOperationException()
        {
            Assert.That(() => _penaltyCalculator.CalculatePenalty(_today, _yesterday, _price), Throws.InvalidOperationException);
        }

        [Test]
        public void CalculatePenalty_ReturnDateWithin2WeeksAfterBorrowDate_ReturnsZero()
        {
            var result = _penaltyCalculator.CalculatePenalty(_yesterday, _today, _price);
            Assert.That(result, Is.Zero);
        }

        [Test]
        public void CalculatePenalty_ReturnDateMoreThan2WeeksAfterBorrowDate_ReturnsPenalty()
        {
            var result = _penaltyCalculator.CalculatePenalty(_threeWeeksAgo, _today, _price);
            Assert.That(result, Is.EqualTo(10*0.01*7));
        }
    }
}
