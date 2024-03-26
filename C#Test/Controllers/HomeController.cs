using C_Test.Models;
using C_Test.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
//using SixLabors.ImageSharp.Formats.Png;
//using SixLabors.ImageSharp.PixelFormats;
using System.Drawing;
using System.Drawing.Imaging;

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
            string apiKey = "vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
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

        public async Task<IActionResult> PieChart()
        {
            string apiKey = "vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
            string jsonData = await ApiService.GetEmployeeData(apiKey);

            if (jsonData != null)
            {
                var employees = JsonConvert.DeserializeObject<List<Employee>>(jsonData);

                var groupedEmployees = employees.GroupBy(e => e.EmployeeName);
                var uniqueEmployees = groupedEmployees.Select(group => group.First()).ToList();

                // Calculate total time worked for each employee
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

                
                using (Bitmap bitmap = new Bitmap(500, 300))
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    
                    graphics.Clear(Color.White);

                    
                    var colors = GetRandomColors(uniqueEmployees.Count);

                    
                    int startAngle = 0;
                    int totalHours = uniqueEmployees.Sum(e => e.TotalTimeWorked);
                    foreach (var (employee, color) in uniqueEmployees.Zip(colors, (emp, c) => (emp, c)))
                    {
                        int sweepAngle = (int)Math.Round((double)employee.TotalTimeWorked / totalHours * 360);
                        using (var brush = new SolidBrush(color))
                        {
                            graphics.FillPie(brush, new Rectangle(10, 10, 200, 200), startAngle, sweepAngle);
                        }
                        startAngle += sweepAngle;
                    }

                    
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pieChart.png");
                    bitmap.Save(imagePath, ImageFormat.Png);
                }

                
                return File("pieChart.png", "image/png");
            }
            else
            {
                return View("Error");
            }
        }



        private List<Color> GetRandomColors(int count)
        {
            var random = new Random();
            var colors = new List<Color>();
            for (int i = 0; i < count; i++)
            {
                colors.Add(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
            }
            return colors;
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