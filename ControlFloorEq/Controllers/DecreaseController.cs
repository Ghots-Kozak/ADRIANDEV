using Abixe_Logic;
using Abixe_Models;
using Abixe_SapServiceLayer;
using Control_Piso.Models;
using ControlFloor.Models;
using ControlFloor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ControlFloor.Controllers
{
    [Route("Decrease")]
    [Authorize]
    public class DecreaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;
        private readonly SapDecrease _sapDecrease;
        public DecreaseController(ApplicationDbContext context, TokenService tokenService, SapDecrease sapDecrease)
        {
            _context = context;
            _tokenService = tokenService;
            _sapDecrease = sapDecrease;
        }

        [HttpGet("Type_Decrease")]
        public async Task<ActionResult> Get_Type_Decrease()
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxDecrease abxDecrease = new AbxDecrease(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await abxDecrease.Get_Type_Decrease(userId);

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
                    throw new Exception("No se encontraron tipos .");
                }
            }
            catch (Exception ex)
            {
                // Maneja excepciones y devuelve un error interno del servidor
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpGet("Decrease")]
       
        public async Task<ActionResult> Get_Decrease(int DocEntry)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxDecrease abxDecrease = new AbxDecrease(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await abxDecrease.Get_Decrease(userId, DocEntry);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return Ok(resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    return NotFound("No se encontraron  productos.");
                }
            }
            catch (Exception ex)
            {
                // Maneja excepciones y devuelve un error interno del servidor
                return StatusCode(500, $"Error al obtener las mermas: {ex.Message}");
            }
        }

        [HttpPost("Decrease")]
        public async Task<IActionResult> Add_Decrease([FromBody] Decrease decrease)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            try
            {

                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                try
                {
                    SapResponse oSapRs = null;

                    if (decrease != null)
                    {
                        AbxDecrease abxDecrease = new AbxDecrease(_context);
                        // Convierte la instancia a JSON
                        string json = JsonConvert.SerializeObject(decrease, Formatting.Indented);
                        //Console.WriteLine(json);

                        var resultadoJson = await abxDecrease.Add_DecreaseLine(userId, json);

                        if (resultadoJson != null)
                        {
                            Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                            string jsonSAP = JsonConvert.SerializeObject(resultadoJson, Formatting.Indented);

                            var jsnVal = JsonConvert.DeserializeObject<Pallets>(jsonSAP);


                            if (jsnVal.DocEntry == 0)
                                oSapRs = await _sapDecrease.Create_DecreaseRequestsAsync(jsonSAP);
                            else
                                oSapRs = await _sapDecrease.Patch_DecreaseRequestsAsync(jsnVal.DocEntry, resultadoJson);


                            if (!oSapRs.IsError)
                            {
                                var sapResp = JsonConvert.DeserializeObject<Pallets>(oSapRs.JsonRsp.ToString());
                                // result.JsonRsp = sapResp;
                                oResp.IsError = false;

                                oResp.Message = "Merma agregada, exitosamente.";
                                oResp.JsonRsp = sapResp;
                                if (sapResp != null)
                                    oResp.EntryObject = sapResp.DocNum;
                                else
                                    oResp.EntryObject = jsnVal.DocNum;

                                return Ok(oResp);
                                // 
                            }
                            else
                            {
                                oResp.IsError = true;
                                oResp.Message = oSapRs.Message;
                                oResp.JsonRsp = oSapRs.JsonRsp;
                                return BadRequest(oResp);

                            }
                        }
                        else
                        {
                            throw new Exception("No se Recupero la estructura dl Pallet Sql");

                        }
                    }
                    else
                    {
                        oResp.IsError = true;
                        oResp.Message = "No cuenta con la estructura correcta";
                        oResp.JsonRsp = "";
                        return BadRequest(oResp);

                    }

                }
                catch (Exception ex)
                {
                    oResp.IsError = true;
                    oResp.Message = "Nose Agrego la maquina el documento. " + ex.Message;
                    return BadRequest(oResp);
                }
                //return Ok(resultadoJson);

            }
            catch (Exception ex)
            {
                oResp.IsError = true;
                oResp.Message = $"Error al Add_MachineOrUpdate: {ex.Message}";
                return StatusCode(500, oResp);
            }

        }


        [HttpPatch("DelDecrease")]
        public async Task<ActionResult> DelDecrease(string DocEntry, int IdLine)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            try
            {

                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });

                try
                {
                    AbxDecrease abxDecrease = new AbxDecrease(_context);
                    // Convierte la instancia a JSON

                    var resultadoJson = await abxDecrease.Delete_Decrease(userId, DocEntry, IdLine);

                    if (resultadoJson)
                    {
                        oResp.IsError = false;
                        oResp.Message = "Eliminado satisfactoriamente.";
                        oResp.JsonRsp = null;
                        oResp.EntryObject = DocEntry.ToString();
                        return Ok(oResp);
                        // oResp.EntryObject = sapResp.DocumentNumber.ToString();
                    }
                    else
                    {
                        throw new Exception("Error de elimino." + _context.Error);

                    }

                }
                catch (Exception ex)
                {
                    throw new Exception ("Nose elimino. " + ex.Message);
                    
                }
                //return Ok(resultadoJson);

            }
            catch (Exception ex)
            {
                oResp.IsError = true;
                oResp.Message = $"Error al delete: {ex.Message}";
                return StatusCode(500, oResp);
            }

        }

    }
}
