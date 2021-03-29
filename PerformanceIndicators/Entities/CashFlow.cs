namespace PerformanceIndicators.Entities
{
    public class CashFlow
    {
        public int Id { get; set; }

        public int? ProjectId { get; set; }

        public int Month { get; set; }

        public decimal Sum { get; set; }

        public Project Project { get; set; }
    }
}
