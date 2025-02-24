﻿namespace C_Test.Models
{
    public class Employee
    {
        public string Id { get; set; }
        public string EmployeeName { get; set; } 
        public int TotalTimeWorked { get; set; }
        public DateTime StarTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; } 
        public string EntryNotes { get; set; } 
        public DateTime? DeletedOn { get; set; } 
    }
}
