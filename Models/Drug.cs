using System;
using System.Collections.Generic;

namespace Yamaha_yte.Models
{
    public partial class Drug
    {
        public Drug()
        {
            Prescriptions = new HashSet<Prescription>();
        }

        public int DrugId { get; set; }
        public string? CodeDrug { get; set; }
        public string? NameDrug { get; set; }
        public string? Content { get; set; }
        public string? Unit { get; set; }
        public int? Number { get; set; }
        public string? Guide { get; set; }

        public virtual ICollection<Prescription> Prescriptions { get; set; }
    }
}
