namespace Library
{
    public interface IPenaltyCalculator
    {
        double CalculatePenalty(DateTime borrowedDate, DateTime returnDate, double price);
    }

    public class PenaltyCalculator : IPenaltyCalculator
    {

        public double CalculatePenalty(DateTime borrowedDate, DateTime returnDate, double price)
        {
            if (returnDate < borrowedDate)
            {
                throw new InvalidOperationException("Data de returnare nu poate fi inaintea imprumutului!");
            }

            double penalty = 0;
            TimeSpan borrowedFor = returnDate - borrowedDate;
            int penaltyDays = borrowedFor.Days - 14;

            if (penaltyDays > 0)
            {
                penalty = price * 0.01 * penaltyDays;
            }

            return penalty;
        }
    }
}