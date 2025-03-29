using System.Data.SqlClient;
using System.Diagnostics;
using Abixe_Logic;
using System.Security.Claims;
using Control_Piso.Models;
using ControlFloor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlFloor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;




        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("Home/getCharts")]
        public async Task<IActionResult> GetCharts()
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxGraficos abxWorkOrders = new AbxGraficos(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await abxWorkOrders.Get_Charts(userId);

              return Ok(resultadoJson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los graficos: {ex.Message}");
            }

        }

        [HttpGet("Home/getChartData/{id}")]
        public async Task<IActionResult> GetChartData(string  id)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxGraficos abxWorkOrders = new AbxGraficos(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await abxWorkOrders.Get_Charts_ById(userId, id);

                return Ok(resultadoJson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el grafico: {ex.Message}");
            }

        }


    }
}
