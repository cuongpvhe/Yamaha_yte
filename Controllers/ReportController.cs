using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yamaha_yte.Models;

namespace Yamaha_yte.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly YAMAHA_YTEContext _context;

        public ReportController(YAMAHA_YTEContext context)
        {
            _context = context;
        }


        [HttpGet("generate-excel/{empId}")]
        public IActionResult GenerateExcel(int empId)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmpId == empId);
            if ( employee == null)
                return NotFound();
            var accident = _context.Accidents.FirstOrDefault(a => a.EmpId == empId);
            var injury = _context.Injuries.FirstOrDefault(i => i.EmpId == empId);  
            var prescription = _context.Prescriptions.Include(p => p.Drug).FirstOrDefault(p => p.EmpId == empId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");

            worksheet.Cell("A1").Value = "PHIẾU CẤP THUỐC";
            DateTime accidentDate = (DateTime)accident.Date; 
            string formattedDate = accidentDate.ToString("yyyy-MM-dd");
            worksheet.Cell("A2").Value = "Ngày khám: " + formattedDate;

            // Assuming 'accident.Hour' is a TimeSpan?
            TimeSpan? accidentHour = accident.Hour;

            if (accidentHour.HasValue)
            {
                // Format the TimeSpan to hh:mm
                string formattedHour = accidentHour.Value.ToString(@"hh\:mm");
                worksheet.Cell("A3").Value = "Giờ khám: " + formattedHour;
            }
            else
            {
                worksheet.Cell("A3").Value = "Giờ khám: N/A"; // Handle the case where Hour is null
            }


            worksheet.Cell("A5").Value = "Mã nhân viên: " + employee.EmpId;
            worksheet.Cell("B5").Value = "Họ tên: " + employee.FullName;
            worksheet.Cell("C5").Value = "Bộ phận: " + employee.DepName;

            worksheet.Cell("A6").Value = "Giới tính: " + employee.Genre;
            DateTime employeeBirth = (DateTime)(employee.Birth);
            string formattedBirth = employeeBirth.ToString("yyyy-MM-dd");
            worksheet.Cell("B6").Value = "Ngày sinh: " + formattedBirth;
            worksheet.Cell("C6").Value = "Nghề nghiệp: " + employee.Job;

            worksheet.Cell("A8").Value = "Chẩn đoán bệnh";
            worksheet.Cell("B8").Value = injury.Code;
            worksheet.Cell("C8").Value = injury.NameInjury;

            worksheet.Cell("A10").Value = "Phương pháp điều trị";
            worksheet.Cell("A12").Value = "Mã số thuốc";
            worksheet.Cell("B12").Value = "Tên thuốc";
            worksheet.Cell("C12").Value = "HL";
            worksheet.Cell("D12").Value = "ĐVT";
            worksheet.Cell("E12").Value = "Số lượng";
            worksheet.Cell("F12").Value = "Hướng dẫn sử dụng";

            worksheet.Cell("A13").Value = prescription.Drug.CodeDrug;
            worksheet.Cell("B13").Value = prescription.Drug.NameDrug;
            worksheet.Cell("C13").Value = prescription.Drug.Content;
            worksheet.Cell("D13").Value = prescription.Drug.Unit;
            worksheet.Cell("E13").Value = prescription.Drug.Number;
            worksheet.Cell("F13").Value = prescription.Drug.Guide;

            worksheet.Cell("A15").Value = "Người khám (Ký tên)";
            worksheet.Cell("D15").Value = "Người nhận thuốc (Ký tên)";

            worksheet.Cell("A16").Value = "Bác sĩ: " + prescription.Doctor;
            worksheet.Cell("D16").Value = "Bệnh nhân: " + employee.FullName;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
        }
    }


}
