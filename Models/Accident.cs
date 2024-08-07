using System;
using System.Collections.Generic;

namespace Yamaha_yte.Models
{
    public partial class Accident
    {
        public int AccidentId { get; set; }
        public int? EmpId { get; set; }
        public string? VolumnCode { get; set; }
        public string? Accident1 { get; set; }
        public string? TransferHospital { get; set; }
        public string? Treatment { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Hour { get; set; }

        public virtual Employee? Emp { get; set; }
    }
}
