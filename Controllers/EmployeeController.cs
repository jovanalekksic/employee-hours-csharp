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

            

            

            
            return View(aggregatedData);

        }
        
       
    }
}