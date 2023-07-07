using AnotherPithonManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using System.Transactions;

namespace AnotherPithonManager.Pages
{
    public class HomeModel : PageModel
    {
        
        private readonly MyDatabaseContext _context;
        public User currentUser;
        public HomeModel(MyDatabaseContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            if (HttpContext.Session.GetString("logged") != "true")
            {
                Redirect("Index");
            }
            currentUser = _context.Users.Where(u => u.Id == HttpContext.Session.GetInt32("id")).First();
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.SetString("logged", "false");
            return Redirect("Index");
        }
        public IActionResult OnPostQuickSearch(int id)
        {
            var matchingRows = currentUser.Volunteers.Where(v => v.Id == id).ToList();
            ViewData["MatchingRows"] = matchingRows;
            return Page();
        }
        public IActionResult OnPostUploadExcel(IFormFile excelFile)
        {
            if (excelFile != null && excelFile.Length > 0)
            {
                using (var package = new ExcelPackage(excelFile.OpenReadStream()))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet

                    List<Volunteer> volunteers = new List<Volunteer>();

                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        Volunteer volunteer = new Volunteer
                        {
                            Id = int.Parse(worksheet.Cells[row, 1].Value.ToString()),
                            Name = worksheet.Cells[row, 2].Value.ToString(),
                            Entry = worksheet.Cells[row, 3].Value.ToString(),
                            Exit = worksheet.Cells[row, 4].Value.ToString(),
                            Date = worksheet.Cells[row, 5].Value.ToString()
                        };

                        volunteers.Add(volunteer);
                    }

                    // Save the volunteers to your database or perform any other required operations
                    currentUser.Volunteers.AddRange(volunteers);
                    _context.SaveChanges();

                    return RedirectToPage("/Volunteers");
                    
                }
            }

            return RedirectToPage("/Index");
        }
        public IActionResult OnGetExportToExcel()
        {
            List<Volunteer> volunteers = currentUser.Volunteers.ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Volunteers");

                worksheet.Cells[1, 1].Value = "תעודת זהות";
                worksheet.Cells[1, 2].Value = "שם מלא";
                worksheet.Cells[1, 3].Value = "שעת כניסה";
                worksheet.Cells[1, 4].Value = "שעת יציאה";
                worksheet.Cells[1, 5].Value = "תאריך";

                for (int i = 0; i < volunteers.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = volunteers[i].Id;
                    worksheet.Cells[i + 2, 2].Value = volunteers[i].Name;
                    worksheet.Cells[i + 2, 3].Value = volunteers[i].Entry;
                    worksheet.Cells[i + 2, 4].Value = volunteers[i].Exit;
                    worksheet.Cells[i + 2, 5].Value = volunteers[i].Date;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set the response headers for downloading the file
                MemoryStream stream = new MemoryStream(package.GetAsByteArray());
                Response.Headers.Add("Content-Disposition", "attachment; filename=Volunteers.xlsx");
                Response.Headers.Add("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}