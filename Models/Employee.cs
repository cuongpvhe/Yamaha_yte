using System;
using System.Collections.Generic;

namespace Yamaha_yte.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Accidents = new HashSet<Accident>();
            Injuries = new HashSet<Injury>();
            Prescriptions = new HashSet<Prescription>();
        }

        public int EmpId { get; set; }
        public string? FullName { get; set; }
        public string? Genre { get; set; }
        public DateTime? Birth { get; set; }
        public string? DepName { get; set; }
        public string? Job { get; set; }

        public virtual ICollection<Accident> Accidents { get; set; }
        public virtual ICollection<Injury> Injuries { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
    }
}
