using System;
using System.Collections.Generic;

namespace Yamaha_yte.Models
{
    public partial class Prescription
    {
        public int PrescriptionId { get; set; }
        public int? EmpId { get; set; }
        public string? Doctor { get; set; }
        public int? DrugId { get; set; }

        public virtual Drug? Drug { get; set; }
        public virtual Employee? Emp { get; set; }
    }
}
