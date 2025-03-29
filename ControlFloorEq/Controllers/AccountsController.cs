using Abixe_Logic;
using Abixe_Models;
using Abixe_Models.DTOs;
using Control_Piso.Models;
using ControlFloor.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
//using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace ControlFloor.Controllers
{
    public class AccountsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public AccountsController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Buscará la vista en "Views/Accounts/Login.cshtml"
        }


        [HttpPost("Login")]
        public async Task<ActionResult> Login( Login credencialesUsuario)
        {
            if (ModelState.IsValid)
            {
   
                if (!string.IsNullOrWhiteSpace(credencialesUsuario.Usuario ) && !string.IsNullOrWhiteSpace(credencialesUsuario.Password))
                {
                    AbxUser User = new AbxUser(_context);
                    var autorizacion = await User.LoginQuery(new AbxAuthorization() { Usuario = credencialesUsuario.Usuario, Password = credencialesUsuario.Password });

                    if (autorizacion != null)
                    {
                        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, Convert.ToString(autorizacion.IdUsuario)),
                    new Claim(ClaimTypes.Name, Convert.ToString(autorizacion.Usuario)),
                    new Claim(ClaimTypes.Role, autorizacion.IdRole),
                    new Claim("IdUsuario", autorizacion.IdUsuario.ToString())
                   // new Claim("CardCode", validUser.CardCode),
                    };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                           new ClaimsPrincipal(claimsIdentity),
                           new AuthenticationProperties
                           {
                               IsPersistent = false // Configura si la cookie es persistente
                           });

                        if (autorizacion.IdRole == "0")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Correo o contraseña no correcta";
                    }
                }

                //try
                //{
                //    var autorizacion = await User.LoginQuery(new AbxAuthorization() { Usuario = credencialesUsuario.Usuario, Password = credencialesUsuario.Password });

                //    if (autorizacion.Usuario != null && autorizacion.IdUsuario != "")
                //    {
                //        authResponse = await _tokenService.GenerarToken(autorizacion);
                //        authResponse.Authorization = autorizacion;
                //        return Ok(authResponse);
                //    }

                //    return Unauthorized(authResponse);
                //}
                //catch (Exception ex)
                //{
                //    // Manejamos cualquier excepción inesperada
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error interno del servidor", Details = ex.Message });
                //}
            }

            return View("Login");
        }
               
        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignOutAsync();

            HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            HttpContext.Response.Headers["Pragma"] = "no-cache";
            HttpContext.Response.Headers["Expires"] = "0";

            //return RedirectToAction("Logout", "Accounts"); // Redirigir al usuario a la página de login
            return View("Login");
        }

    }
}
