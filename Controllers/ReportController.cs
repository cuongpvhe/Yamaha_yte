using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yamaha_yte.Models;
using Yamaha_yte.Dto;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        // Phương thức GET để tạo báo cáo từ dữ liệu trong cơ sở dữ liệu
        [HttpGet("generate-excel/{empId}")]
        public IActionResult GenerateExcel(int empId)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmpId == empId);
            if (employee == null)
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

            TimeSpan? accidentHour = accident.Hour;
            worksheet.Cell("A3").Value = "Giờ khám: " + (accidentHour.HasValue ? accidentHour.Value.ToString(@"hh\:mm") : "N/A");

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

            // Chèn ảnh vào worksheet
            using var imageStream = new FileStream("D:\\Yamaha_yte\\yamaha.png", FileMode.Open, FileAccess.Read);
            var picture = worksheet.AddPicture(imageStream);
            picture.MoveTo(worksheet.Cell("H1"));
            picture.Width = 100;  // Thay đổi chiều rộng
            picture.Height = 100; // Thay đổi chiều cao

            // Phủ nền trắng lên vùng dữ liệu
            var rangeToOverlay = worksheet.Range("A1:F16");
            rangeToOverlay.Style.Fill.BackgroundColor = XLColor.White;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
        }

        // Phương thức POST để tạo báo cáo từ dữ liệu và hình ảnh tải lên
        [HttpPost("create-report")]
        public async Task<IActionResult> CreateReport([FromForm] ReportDataDTO reportData)
        {
            // Validate input
            if (reportData == null || reportData.Image == null || reportData.Image.Length == 0)
                return BadRequest("Invalid data or image.");

            // Insert data into the database
            var employee = new Employee
            {
                EmpId = reportData.EmpId,
                FullName = reportData.FullName,
                Genre = reportData.Genre,
                Birth = reportData.Birth,
                DepName = reportData.DepName,
                Job = reportData.Job
            };

            var accident = new Accident
            {
                EmpId = reportData.EmpId,
                VolumnCode = "VC001", // Use a default value or generate based on your logic
                Accident1 = "Accident details", // Use default or from input
                TransferHospital = "Transfer Hospital", // Use default or from input
                Treatment = "Treatment details", // Use default or from input
                Date = reportData.AccidentDate,
                Hour = reportData.AccidentHour
            };

            var injury = new Injury
            {
                EmpId = reportData.EmpId,
                Code = reportData.InjuryCode,
                NameInjury = reportData.InjuryName
            };

            var drug = new Drug
            {
                CodeDrug = reportData.DrugCode,
                NameDrug = reportData.DrugName,
                Content = reportData.DrugContent,
                Unit = reportData.DrugUnit,
                Number = reportData.DrugNumber,
                Guide = reportData.DrugGuide
            };

            var prescription = new Prescription
            {
                EmpId = reportData.EmpId,
                Doctor = reportData.Doctor,
                DrugId = drug.DrugId // Ensure this ID is set properly
            };

            _context.Employees.Add(employee);
            _context.Accidents.Add(accident);
            _context.Injuries.Add(injury);
            _context.Drugs.Add(drug);
            _context.Prescriptions.Add(prescription);

            await _context.SaveChangesAsync();

            // Generate Excel report
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");

            worksheet.Cell("A1").Value = "PHIẾU CẤP THUỐC";
            worksheet.Cell("A2").Value = "Ngày khám: " + reportData.AccidentDate.ToString("yyyy-MM-dd");
            worksheet.Cell("A3").Value = "Giờ khám: " + (reportData.AccidentHour.HasValue ? reportData.AccidentHour.Value.ToString(@"hh\:mm") : "N/A");

            worksheet.Cell("A5").Value = "Mã nhân viên: " + reportData.EmpId;
            worksheet.Cell("B5").Value = "Họ tên: " + reportData.FullName;
            worksheet.Cell("C5").Value = "Bộ phận: " + reportData.DepName;

            worksheet.Cell("A6").Value = "Giới tính: " + reportData.Genre;
            worksheet.Cell("B6").Value = "Ngày sinh: " + reportData.Birth.ToString("yyyy-MM-dd");
            worksheet.Cell("C6").Value = "Nghề nghiệp: " + reportData.Job;

            worksheet.Cell("A8").Value = "Chẩn đoán bệnh";
            worksheet.Cell("B8").Value = reportData.InjuryCode;
            worksheet.Cell("C8").Value = reportData.InjuryName;

            worksheet.Cell("A10").Value = "Phương pháp điều trị";
            worksheet.Cell("A12").Value = "Mã số thuốc";
            worksheet.Cell("B12").Value = "Tên thuốc";
            worksheet.Cell("C12").Value = "HL";
            worksheet.Cell("D12").Value = "ĐVT";
            worksheet.Cell("E12").Value = "Số lượng";
            worksheet.Cell("F12").Value = "Hướng dẫn sử dụng";

            worksheet.Cell("A13").Value = reportData.DrugCode;
            worksheet.Cell("B13").Value = reportData.DrugName;
            worksheet.Cell("C13").Value = reportData.DrugContent;
            worksheet.Cell("D13").Value = reportData.DrugUnit;
            worksheet.Cell("E13").Value = reportData.DrugNumber;
            worksheet.Cell("F13").Value = reportData.DrugGuide;

            worksheet.Cell("A15").Value = "Người khám (Ký tên)";
            worksheet.Cell("D15").Value = "Người nhận thuốc (Ký tên)";

            worksheet.Cell("A16").Value = "Bác sĩ: " + reportData.Doctor;
            worksheet.Cell("D16").Value = "Bệnh nhân: " + reportData.FullName;

            // Chèn ảnh vào worksheet
            using var imageStream = reportData.Image.OpenReadStream();
            var picture = worksheet.AddPicture(imageStream);
            picture.MoveTo(worksheet.Cell("H1"));
            picture.Width = 100;  // Thay đổi chiều rộng
            picture.Height = 100; // Thay đổi chiều cao

            // Phủ nền trắng lên vùng dữ liệu
            var rangeToOverlay = worksheet.Range("A1:F16");
            rangeToOverlay.Style.Fill.BackgroundColor = XLColor.White;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
        }
    }
}
