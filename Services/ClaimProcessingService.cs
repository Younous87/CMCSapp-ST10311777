using ClosedXML.Excel;
using CMCSapp_ST10311777.Models;

namespace CMCSapp_ST10311777.Services
{
    public class ClaimProcessingService
    {
        private readonly ClaimTable _claimTable;
        private readonly DocumentTable _documentTable;
        private readonly ILogger<ClaimProcessingService> _logger;
        private readonly IConfiguration _configuration;

        public ClaimProcessingService(
            ClaimTable claimTable,
            DocumentTable documentTable,
            ILogger<ClaimProcessingService> logger,
            IConfiguration configuration)
        {
            _claimTable = claimTable;
            _documentTable = documentTable;
            _logger = logger;
            _configuration = configuration;
        }

        // Generate comprehensive payment report
        public byte[] GeneratePaymentReport(DateTime startDate, DateTime endDate)
        {
            // Retrieve approved claims within the date range
            var approvedClaims = _claimTable.GetAllClaims()
                .Where(c => c.status == "Approved" || c.status == "Auto-Approved" &&
                       c.claimDate >= startDate &&
                       c.claimDate <= endDate)
                .ToList();

            // Group claims by lecturer
            var groupedClaims = approvedClaims
                .GroupBy(c => new { c.claimID })
                .Select(g => new
                {
                    LecturerName = g.Key.claimID,
                    TotalHours = g.Sum(c => c.hoursWorked),
                    TotalAmount = g.Sum(c => c.totalAmount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            // Generate Excel report
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Payment Report");

                // Report Headers
                worksheet.Cell(1, 1).Value = "Payment Report";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(2, 1).Value = $"Period: {startDate.ToShortDateString()} - {endDate.ToShortDateString()}";

                // Column Headers
                worksheet.Cell(4, 1).Value = "Claim ID";
                worksheet.Cell(4, 2).Value = "Hours Worked";
                worksheet.Cell(4, 3).Value = "Total Amount";

                // Populate data
                for (int i = 0; i < groupedClaims.Count; i++)
                {
                    worksheet.Cell(i + 5, 1).Value = groupedClaims[i].LecturerName;
                    worksheet.Cell(i + 5, 2).Value = groupedClaims[i].TotalHours;
                    worksheet.Cell(i + 5, 3).Value = groupedClaims[i].TotalAmount;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save to memory stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
