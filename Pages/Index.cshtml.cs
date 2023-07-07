using AnotherPithonManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnotherPithonManager.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<HomeModel> _logger;
        private readonly MyDatabaseContext _context;
        public List<User> Users { get; set; } = new List<User>();

        [BindProperty]
        public User NewUser { get; set; }

        public IndexModel(MyDatabaseContext context, ILogger<HomeModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        public void OnGet()
        {
            if (HttpContext.Session.GetString("logged") == null)
            {
                HttpContext.Session.SetString("logged", "false");
            }
            else if (HttpContext.Session.GetString("logged") == "false")
            {

            }
            else if (HttpContext.Session.GetString("logged") == "true")
            {
                Redirect("Home");
            }
        }

        public IActionResult OnPostRegister()
        {
            if(!(_context.Users.Any(u => u.UserEmail == NewUser.UserEmail)))
            {
                User e = NewUser;
                _context.Users.Add(NewUser);
                _context.SaveChanges();
                int id = _context.Users.First(u => u.UserEmail == e.UserEmail).Id;
                HttpContext.Session.SetInt32("id", id);
                HttpContext.Session.SetString("logged", "true");
                return Redirect("Home");
            }
            else
            {
                return Content(@"<div class=""alert danger-warning alert-dismissible fade show"" role=""alert"">
                      <strong>משתמש קיים עם אימייל זה</strong>
                      <button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""Close"">
                        <span aria-hidden=""true"">&times;</span>
                      </button>
                    </div>");
            }
        }

        public IActionResult OnPostLogin()
        {
            if (_context.Users.Any(u => u.UserEmail == NewUser.UserEmail))
            {
                if(_context.Users.Where(u => u.UserEmail == NewUser.UserEmail).First().UserPassword == NewUser.UserPassword)
                {
                    int id = _context.Users.Where(u => u.UserEmail == NewUser.UserEmail).First().Id;
                    HttpContext.Session.SetInt32("Id", id);
                    HttpContext.Session.SetString("logged", "true");
                    return Redirect("Home");
                }
                else
                {
                    return Page();
                }
            }
            else
            {
                return Page();
            }
        }
    }
}

/*
                    return Content(@"<div class=""alert danger-warning alert-dismissible fade show"" role=""alert"">
                      <strong>אימייל או סיסמה שגויים</strong>
                      <button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""Close"">
                        <span aria-hidden=""true"">&times;</span>
                      </button>
                    </div>");*/
