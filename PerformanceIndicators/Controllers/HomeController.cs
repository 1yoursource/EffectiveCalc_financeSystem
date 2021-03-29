using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PerformanceIndicators.Entities;
using PerformanceIndicators.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace PerformanceIndicators.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext _db;

        public HomeController(ApplicationContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            Project project = _db.Projects.Include(p => p.CashFlows).FirstOrDefault();
            ProjectViewModel projectViewModel = new ProjectViewModel
            {
                Name = string.Empty,
                Duration = 0,
                DiscountRate = 0.0M,
                Investments = 0.0M
            };

            if (project != null)
            {
                projectViewModel.Name = project.Name;
                projectViewModel.Duration = project.Duration;
                projectViewModel.DiscountRate = project.DiscountRate;
                projectViewModel.Investments = project.Investments;
                if (project.CashFlows != null && project.CashFlows.Count() == project.Duration)
                    projectViewModel.CashFlows = project.CashFlows
                        .Select(cf => new CashFlowViewModel { Month = cf.Month, Sum = cf.Sum })
                        .OrderBy(cf => cf.Month)
                        .ToArray();
                else
                {
                    projectViewModel.CashFlows = new CashFlowViewModel[project.Duration];
                }
            }

            return View(projectViewModel);
        }

        [HttpPost]
        public IActionResult Index(ProjectViewModel projectViewModel)
        {
            var project = new Project
            {
                Name = projectViewModel.Name,
                Duration = projectViewModel.Duration,
                DiscountRate = projectViewModel.DiscountRate,
                Investments = projectViewModel.Investments
            };
            var cashFlows = projectViewModel.CashFlows?
                    .Select(cf => new CashFlow { Sum = cf.Sum, Month = cf.Month, Project = project });
            if (_db.Projects.Count() == 0)
            {
                _db.Projects.Add(project);
                if (cashFlows != null)
                    _db.CashFlows.AddRange(cashFlows);
            }
            else
            {
                project = _db.Projects.FirstOrDefault();
                project.Name = projectViewModel.Name;
                project.Duration = projectViewModel.Duration;
                project.DiscountRate = projectViewModel.DiscountRate;
                project.Investments = projectViewModel.Investments;
                if (cashFlows != null)
                {
                    _db.CashFlows.RemoveRange(_db.CashFlows);
                    _db.CashFlows.AddRange(cashFlows);
                }
            }

            _db.SaveChanges();
            if (projectViewModel.CashFlows != null)
                projectViewModel.CashFlows = projectViewModel.CashFlows?.OrderBy(cf => cf.Month).ToArray();
            else
                projectViewModel.CashFlows = new CashFlowViewModel[projectViewModel.Duration];

            return View(projectViewModel);
        }

        public IActionResult Result(ProjectViewModel projectViewModel)
        {
            var cashFlows = projectViewModel.CashFlows.Select(cf => cf.Sum).ToArray();
            var result = new ResultViewModel
            {
                Npv = GetNpv(projectViewModel.Investments, projectViewModel.DiscountRate, cashFlows),
                Pi = GetPi(projectViewModel.Investments, projectViewModel.DiscountRate, cashFlows)
            };

            return View(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private decimal GetNpv(decimal investments, decimal discountRate, decimal[] cashFlows)
        {
            decimal npv = 0.0M;
            discountRate /= 100.0M;

            for (int i = 1; i <= cashFlows.Length; i++)
                npv += cashFlows[i - 1] / (decimal)Math.Pow((double)(1.0M + discountRate), i - 1);

            npv -= investments;
            return npv;
        }

        private decimal GetPi(decimal investments, decimal discountRate, decimal[] cashFlows)
        {
            decimal pi = 0.0M;
            discountRate /= 100.0M;

            for (int i = 1; i <= cashFlows.Length; i++)
                pi += cashFlows[i - 1] / (decimal)Math.Pow((double)(1.0M + discountRate), i - 1);

            pi /= investments;
            return pi;
        }
    }
}
