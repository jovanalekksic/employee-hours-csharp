using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeHours.Models
{
    public class Employee
    {
        public string Id { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StarTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public  DateTime? DeletedOn { get; set; }
        public double TotalHours { get; set; }
        public double Percentage { get; set; }
    }
}