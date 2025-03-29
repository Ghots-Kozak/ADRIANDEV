using Abixe_Logic;
using Abixe_Models;
using Abixe_SapServiceLayer;
using ControlFloor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Claims;
using Control_Piso.Models;
using ControlFloor.Services;
using Humanizer;

namespace ControlFloor.Controllers
{
    
    [Authorize]
    [Route("Pallets")]
    public class PalletsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SapPallets _sapPallets;


        public PalletsController(ApplicationDbContext context, TokenService tokenService, SapPallets pallets)
        {
            _context = context;
            _sapPallets = pallets;
        }


        [HttpGet("IdPallet")]
        public async Task<IActionResult> Pallets()
        {
            ResponseAbx oResp = new ResponseAbx { IsError = true };

            try
            {
                // 🔹 Validar si el usuario está autenticado
                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });


                // 🔹 Crear la instancia de Pallets
                Pallets pallets = new Pallets { Pallet = "" };

                if (pallets == null)
                {
                    oResp.Message = "No cuenta con la estructura correcta";
                    return BadRequest(oResp);
                }

                AbxPallets abxPallets = new AbxPallets(_context);
                string json = JsonConvert.SerializeObject(pallets, Formatting.Indented);
                Console.WriteLine($"Solicitud JSON: {json}");

                // 🔹 Invocar el método asíncrono para agregar el pallet
                var resultadoJson = await abxPallets.Add_PalletsCab(userId, pallets);

                if (resultadoJson == null)
                {
                    throw new Exception("No se recuperó la estructura de la máquina SQL.");
                }

                // 🔹 Imprimir resultado para depuración
                Console.WriteLine($"Resultado Pallet JSON: {JsonConvert.SerializeObject(resultadoJson)}");

                // 🔹 Convertir el resultado a JObject para extraer datos específicos
                var jsonObject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                string folio = jsonObject["Pallet"]?.ToString() ?? "N/A"; // Obtener "Pallet", si es nulo asigna "N/A"

                // 🔹 Construir respuesta exitosa
                oResp.IsError = false;
                oResp.Message = "Pallet generado correctamente";
                oResp.JsonRsp = resultadoJson;
                oResp.EntryObject = folio;

                return Ok(oResp);
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Error al procesar JSON: {jsonEx.Message}");
                oResp.Message = "Error al procesar la respuesta JSON.";
                return StatusCode(500, oResp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                oResp.Message = $"Error al Add_Pallet: {ex.Message}";
                return StatusCode(500, oResp);
            }
        }

        [HttpPost("PalletsLines")]
        public async Task<IActionResult> PalletsLines([FromBody] PalletsLines pallets)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            try
            {

                // 🔹 Validar si el usuario está autenticado
                var user = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(user))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });

                string userId = user;

                try
                {
                    SapResponse oSapRs = null;

                    if (pallets != null )
                    {
                        if (pallets.EntryOf == 0)
                            throw new Exception($"No se enontro OF:{pallets.EntryOf.ToString()}");

                        AbxPallets abxPallets = new AbxPallets(_context);
                        // Convierte la instancia a JSON
                        string json = JsonConvert.SerializeObject(pallets, Formatting.Indented);
                        //Console.WriteLine(json);

                        var resultadoJson = await abxPallets.Add_UDO_PalletsLine(userId, json);

                        if (resultadoJson != null)
                        {
                            Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                            string jsonSAP = JsonConvert.SerializeObject(resultadoJson, Formatting.Indented);
                            var jsnVal = JsonConvert.DeserializeObject<Pallets>(jsonSAP);


                            if (jsnVal.DocEntry == 0)
                            {
                                oSapRs = await _sapPallets.Create_PalletsRequestsAsync(jsonSAP);
                                if (!oSapRs.IsError)
                                {
                                    var sapResp = JsonConvert.DeserializeObject<Pallets>(oSapRs.JsonRsp.ToString());
                                    // result.JsonRsp = sapResp;
                                    oResp.IsError = false;

                                    oResp.Message = "Pallet agregada, exitosamente.";
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
                                //oSapRs = await _sapPallets.Patch_PalletsRequestsAsync(jsnVal.DocEntry, resultadoJson);
                                resultadoJson = await abxPallets.Add_PalletsLineBySQL(userId, json);

                                if (resultadoJson != null)
                                {
                                    oResp.IsError = false;
                                    oResp.Message = "Pallet agregada, exitosamente.";
                                    oResp.JsonRsp = resultadoJson;
                                    oResp.EntryObject = jsnVal.DocNum;

                                    return Ok(oResp);
                                }
                                else
                                {
                                    oResp.IsError = true;
                                    oResp.Message = "Error al registrar el pallet";
                                    oResp.JsonRsp = resultadoJson;
                                    return BadRequest(oResp);
                                }

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

        [HttpPut("Close_Pallets")]
        public async Task<IActionResult> Close_Pallets(string IdPallet)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            try
            {

                var user = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(user))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });

                string userId = user;

                if (IdPallet != null)
                {
                    AbxPallets abxPallets = new AbxPallets(_context);
                    // Convierte la instancia a JSON

                    var resultadoJson = await abxPallets.Close_Pallets(userId, IdPallet);

                    if (resultadoJson != null)
                    {
                        //var jsonObject = JObject.Parse(resultadoJson.ToString());
                        //var folio = jsonObject["Folio"]?.ToString(); // Extrae el folio

                        Console.WriteLine($"Resultado Pallet JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                        var jsonObject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));

                        // Extraer el valor del campo "Pallet"
                        var folio = jsonObject["Pallet"]?.ToString();

                        oResp.IsError = false;
                        oResp.Message = "Pallet";
                        oResp.JsonRsp = resultadoJson;
                        oResp.EntryObject = folio;

                        return Ok(oResp);
                    }
                    else
                    {
                        throw new Exception("No se Recupero la estructura del Pallet Sql");

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
                oResp.Message = $"Error al Add_Pallet: {ex.Message}";
                return StatusCode(500, oResp);
            }

        }


    }
}
