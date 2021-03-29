using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PerformanceIndicators.Models
{
    public class ProjectViewModel
    {
        [Display(Name = "Назва проекту")]
        public string Name { get; set; }

        [Display(Name = "Тривалість проекту в місяцях")]
        public int Duration { get; set; }

        [Display(Name = "Ставка дисконтування")]
        public decimal DiscountRate { get; set; }

        [Display(Name = "Початкові інвестиції")]
        public decimal Investments { get; set; }

        public IEnumerable<CashFlowViewModel> CashFlows { get; set; }
    }
}
