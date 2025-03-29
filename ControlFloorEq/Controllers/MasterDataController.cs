using Abixe_Logic;
using Control_Piso.Models;
using ControlFloor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ControlFloor.Controllers
{
    [Route("MasterData")]
    [Authorize]
    public class MasterDataController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;
        public MasterDataController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpGet("Get_BusinessPartners")]
        public async Task<ActionResult> Get_BusinessPartners(string Type)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_BusinessPartners(userId, Type);

                if (resultadoJson != null)
                {
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return Json(resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron SN para el usuario.");
                }
            }
            catch (Exception ex)
            {
                // Maneja excepciones y devuelve un error interno del servidor
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpGet("Get_WhareHouses")]
        public async Task<ActionResult> Get_WhareHouses()
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });

                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_WhareHouses(userId);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return Json(resultadoJson);

                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron SN para el usuario.");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpGet("Get_Machine")]
        public async Task<ActionResult> Get_Machine(string? ItemCode)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_Machine(userId, ItemCode);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    return Json(resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron Maquinas.");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpGet("Get_Components")]
        public async Task<ActionResult> Get_Components(int GroupCode)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_Components(userId, GroupCode);

                if (resultadoJson != null)
                {
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return Json(resultadoJson);
                }
                else
                {
                    throw new Exception("No se encontraron Componentes.");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpGet("Get_Mezclas")]
        public async Task<ActionResult> Get_Mezclas(string Machine, string ItemCode)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_Mezclas(userId, Machine, ItemCode);

                if (resultadoJson != null)
                {
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return Json(resultadoJson);
                }
                else
                {
                    throw new Exception("No se encontraron Mezclas.");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }



        [HttpGet("Taxs")]
        public async Task<ActionResult> Get_Taxs()
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_Taxs(userId, "C");

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return Json(resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron Impuestos.");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpGet("ItemClassification")]
        public async Task<ActionResult> Get_Itemclassification( string Module)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_Itemclassification(userId, Module);

                if (resultadoJson != null)
                {
                    return Json(resultadoJson);

                }
                else
                {
                    throw new Exception("No se encontraron Clasificaciones.");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpGet("Currencys")]
        public async Task<ActionResult> Get_Currencys()
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_Currencys(userId);

                if (resultadoJson != null)
                {
                    return Json(resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron Monedas.");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpGet("Get_Items")]
        public async Task<ActionResult> Get_Items(int GroupCode, string Module)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_Items(userId, GroupCode, Module);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return Json(resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron articulos.");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });

            }
        }

        [HttpGet("Technicals")]
        public async Task<ActionResult> Get_Technicals()
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_TechNicals(userId);

                if (resultadoJson != null)
                {
                    return Json(resultadoJson);

                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se Tecnicos");
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devolvemos un error en JSON
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        
        

        [HttpGet("Get_Employes_Supervisor")]
        public async Task<ActionResult> Get_Employes(string Type)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });

                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_Employes(userId, Type);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return Json(resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron Employes para el usuario.");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpGet("Get_StopTime_Motivos")]
        public async Task<ActionResult> StopTime_Motivos(string? Type)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });

                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_StopTime_Motivos(userId, Type);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return Json(resultadoJson);

                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron Motivos para el usuario.");
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devolvemos un error en JSON
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }


        [HttpGet("Get_WhareHousesModule")]
        public async Task<ActionResult> Get_WhareHousesModule(string Module)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });

                }
                string userId = userIdClaim.Value;

                AbxMasterData oabxMasterData = new AbxMasterData(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await oabxMasterData.Get_WhareHouses_Module(userId, Module);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return Json(resultadoJson);

                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron SN para el usuario.");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

    }
}
