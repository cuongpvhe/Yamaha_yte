namespace Yamaha_yte.Dto
{
    public class ReportDataDTO
    {
        public int EmpId { get; set; }
        public string FullName { get; set; }
        public string Genre { get; set; }
        public DateTime Birth { get; set; }
        public string DepName { get; set; }
        public string Job { get; set; }
        public DateTime AccidentDate { get; set; }
        public TimeSpan? AccidentHour { get; set; }
        public string InjuryCode { get; set; }
        public string InjuryName { get; set; }
        public string DrugCode { get; set; }
        public string DrugName { get; set; }
        public string DrugContent { get; set; }
        public string DrugUnit { get; set; }
        public int DrugNumber { get; set; }
        public string DrugGuide { get; set; }
        public string Doctor { get; set; }
        public IFormFile Image { get; set; }
    }
}
