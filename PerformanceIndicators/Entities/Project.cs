using System.Collections.Generic;

namespace PerformanceIndicators.Entities
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Duration { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Investments { get; set; }

        public IEnumerable<CashFlow> CashFlows { get; set; }
    }
}
