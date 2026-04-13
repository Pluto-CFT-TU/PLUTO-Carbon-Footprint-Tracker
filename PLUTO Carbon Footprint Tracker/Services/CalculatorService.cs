namespace PLUTO_Carbon_Footprint_Tracker.Services
{
    public class CalculatorService
    {
        public decimal Calculate(decimal quantity, decimal factor)
        {
            return quantity * factor;
        }
    }
}
