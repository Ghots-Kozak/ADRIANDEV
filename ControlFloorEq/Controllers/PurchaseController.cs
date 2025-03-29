using Abixe_Logic;
using Abixe_SapServiceLayer;
using Control_Piso.Models;
using ControlFloor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ControlFloor.Controllers
{
    [Authorize]
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;
    

        public PurchaseController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
        }



        [HttpGet("Purchase/MyPurchase")]
        public async Task<IActionResult> MyPurchase()
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");/// Cargar las Ordenes de fabricacion
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxPurchaseOrders purchaseOrders = new AbxPurchaseOrders(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await purchaseOrders.Get_MyPurchaseOrders(userId);

                if (resultadoJson != null)
                {
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return View("MyPurchase", resultadoJson); // Enviamos los datos a la vista
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron órdenes de compra para el usuario.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message; // Pasar error a la vista
                return View("MyPurchase", null);
            }
        }

        [HttpGet("Purchase/MyPurchasenLineas")]
        public async Task<IActionResult> MyPurchasenLineas(int DocEntry)
        {
            try
            {
                string userId;

                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                    if (userIdClaim == null)
                    {
                        return View("PlnAutorizacion", null); // Vista sin datos
                    }
                    userId = userIdClaim.Value;
                }
                else
                {
                    // En desarrollo, usar un IdUsuario de prueba
                    userId = "DevUser";
                }

                AbxPurchaseOrders purchaseOrders = new AbxPurchaseOrders(_context);
                var resultadoJson = await purchaseOrders.Get_MyPurchaseOrderLineas(userId, DocEntry);


                if (resultadoJson != null)
                {
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return Json(resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron órdenes de  para el usuario.");
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devolvemos un error en JSON
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }


    }
}
