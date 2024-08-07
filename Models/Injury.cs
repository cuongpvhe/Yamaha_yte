using System;
using System.Collections.Generic;

namespace Yamaha_yte.Models
{
    public partial class Injury
    {
        public int InjuryId { get; set; }
        public int? EmpId { get; set; }
        public string? Code { get; set; }
        public string? NameInjury { get; set; }

        public virtual Employee? Emp { get; set; }
    }
}
