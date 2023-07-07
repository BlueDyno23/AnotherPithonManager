using AnotherPithonManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace AnotherPithonManager.Pages
{
    public class VolunteersModel : PageModel
    {
        private readonly MyDatabaseContext _context;
        public List<Volunteer> Volunteers { get; set; } = new List<Volunteer>();

        [BindProperty]
        public Volunteer NewVolunteer { get; set; }



        public VolunteersModel(MyDatabaseContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
            if (HttpContext.Session.GetString("logged") != "true")
            {
                Redirect("Index");
            }
            int userId = (int)HttpContext.Session.GetInt32("id");
            var user = _context.Users.Where(u => u.Id == userId)
                .Include(u => u.Volunteers)
                .FirstOrDefault();
            //Volunteers = _context.Users.FirstOrDefault(u => u.Id == userId).Volunteers;
            Volunteers = user.Volunteers;
            _context.SaveChanges();
        }
        public IActionResult OnPost()
        {
            var userId = (int)HttpContext.Session.GetInt32("id");
            var user = _context.Users.Where(u => u.Id == userId)
                .Include(u => u.Volunteers)
                .FirstOrDefault();

            user.Volunteers.Add(NewVolunteer);
            _context.SaveChanges();

            Volunteers = user.Volunteers;
            return Page();
        }


        public IActionResult OnGetExportToExcel()
        {
            List<Volunteer> volunteers = _context.Users.FirstOrDefault(u => u.Id == HttpContext.Session.GetInt32("id")).Volunteers;
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

                MemoryStream stream = new MemoryStream(package.GetAsByteArray());
                Response.Headers.Add("Content-Disposition", "attachment; filename=Volunteers.xlsx");
                Response.Headers.Add("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}
