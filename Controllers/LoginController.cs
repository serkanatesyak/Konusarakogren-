using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KonusarakOgrenWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly Context _context;
        public LoginController(Context context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Giriş()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Giriş([Bind("UserMail,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                ClaimsIdentity kimlik = null;
                bool kimlikdoğrulandı = false;
                var kullanıcı = await _context.Users.Include(k => k.Rol).FirstOrDefaultAsync(m => m.UserMail == user.UserMail && m.Password == user.Password);
                
                if (kullanıcı == null)
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya Şifre yanlış!");
                    return View(user);
                }

                kimlik = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Sid, user.UserID.ToString()),
                        new Claim(ClaimTypes.Email,user.UserMail),
                        new Claim(ClaimTypes.Role, kullanıcı.Rol.RolName)
                    }, CookieAuthenticationDefaults.AuthenticationScheme
                );

                kimlikdoğrulandı = true;

                if (kimlikdoğrulandı)
                {
                    var ilkeler = new ClaimsPrincipal(kimlik);
                    var giriş = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ilkeler);

                    if (kullanıcı.RolID == 1)
                    {
                        return Redirect("~/Product/Index");
                    }

                    else
                    {
                        return Redirect("~/Product/ProductAdd/");
                    }
                }
            }
            return View();
        }



    }
}
