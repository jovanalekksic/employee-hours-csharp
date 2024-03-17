using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using EmployeeHours.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Web.Helpers;
using System.IO;

namespace EmployeeHours.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly HttpClient _httpClient;

        public EmployeeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public EmployeeController()
        {
            
        }
        
        const string KEY= "vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
        public ActionResult Index()
        {
            var webClient = new WebClient();
            var json = webClient.DownloadString("https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code="+KEY);
            
            //Deserijalizovanje JSON niza u listu zaposlenih
            List<Models.Employee> employees = JsonConvert.DeserializeObject<List<Models.Employee>>(json);

            //nova lista grupisanih podataka
            var aggregatedData = new List<Models.Employee>();

            //Grupisanje zaposlenih po imenu
            foreach (var group in employees.GroupBy(e => e.EmployeeName))
            {
                //Izracunavanje ukupnih sati za svaku grupu
                var totalHours = Math.Round(group.Sum(e => (e.EndTimeUtc - e.StarTimeUtc).TotalHours));

                //Kreiranje posebnog novog zaposlenog za svaku grupu
                var employee = new Models.Employee
                {
                    EmployeeName = group.Key,
                    TotalHours = totalHours,
                    Percentage=0.0
                };
                //Dodavanje novog objekta zaposlenog(sa ukupnim satima)
                aggregatedData.Add(employee);
            }
            //Sortiranje 
            aggregatedData.Sort((a, b) => b.TotalHours.CompareTo(a.TotalHours));

            //procenat za svakog zaposlenog
            var totalHoursSum = aggregatedData.Sum(e => e.TotalHours);
            foreach (var employee in aggregatedData)
            {
                employee.Percentage = Math.Round((employee.TotalHours / totalHoursSum) * 100, 2);
            }

            //priprema podataka za grafik
            var employeeNamesPercentages = aggregatedData.Select(e => $"{e.EmployeeName} ({e.Percentage}%)").ToArray();
            var percentagesChart = new Chart(width: 600, height: 400)
                .AddTitle("Percentage of total hours worked by employee")
                .AddSeries(
                    chartType: "Pie",
                    xValue: employeeNamesPercentages,
                    yValues: aggregatedData.Select(e => e.TotalHours).ToArray()
                );


            //kreiranje png slike
            var chartPng = percentagesChart.GetBytes("png");

            var filePath = Server.MapPath("~/Images/pie-chart.png");
            System.IO.File.WriteAllBytes(filePath, chartPng);




            return View(aggregatedData);

        }
        
       
    }
}