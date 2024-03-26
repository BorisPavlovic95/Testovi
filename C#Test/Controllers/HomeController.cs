using C_Test.Models;
using C_Test.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace C_Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            string apiKey = "vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ=="; // Replace with your API key
            string jsonData = await ApiService.GetEmployeeData(apiKey);

            if (jsonData != null)
            {
                var employees = JsonConvert.DeserializeObject<List<Employee>>(jsonData);

                var groupedEmployees = employees.GroupBy(e => e.EmployeeName);
                var uniqueEmployees = groupedEmployees.Select(group => group.First()).ToList();

                foreach (var group in groupedEmployees)
                {
                    foreach (var employee in group)
                    {
                        double totalHoursWorked = 0;

                        foreach (var emp in group)
                        {
                            DateTime startTime = DateTime.SpecifyKind(emp.StarTimeUtc, DateTimeKind.Utc);
                            DateTime endTime = DateTime.SpecifyKind(emp.EndTimeUtc, DateTimeKind.Utc);
                            TimeSpan timeWorked = endTime - startTime;

                            totalHoursWorked += Math.Round(timeWorked.TotalHours);
                        }             
                        employee.TotalTimeWorked = (int)totalHoursWorked;
                    }
                }

                return View(uniqueEmployees);
            }
            else
            {
                
                return View("Error");
            }
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}