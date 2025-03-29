using Abixe_Logic;
using Abixe_Models;
using Abixe_SapServiceLayer;
using Control_Piso.Models;
using ControlFloor.Models;
using ControlFloor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace ControlFloor.Controllers
{
    
    [Authorize]
    public class ProductionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SapProductionOrderService _productionOrderService;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductionController(ApplicationDbContext context, TokenService tokenService, SapProductionOrderService productionOrderService, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _productionOrderService = productionOrderService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult> ListProductionOrder()
        {
            try
            {
                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });


                AbxProductionOrders AbxProductionOrders = new AbxProductionOrders(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await AbxProductionOrders.Get_Documents_Planeacion(userId, 0, 0);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return View("ListProductionOrder", resultadoJson); // Enviamos los datos a la vista
                }
                else
                {
                  throw new Exception ("El resultado es nulo o vacío.");
                   
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message; // Pasar error a la vista
                return View("ListProductionOrder", null);
            }
        }


        [HttpGet]
        [Route("Production/WPS")]
        public async Task<ActionResult> WPS(int DocEntry)
        {
            try
            {
                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });

                AbxProductionOrders ProductionOrders = new AbxProductionOrders(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await ProductionOrders.Get_Production_Componentes_OP(userId, DocEntry);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return View("WPS", resultadoJson); // Enviamos los datos a la vista
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    return NotFound("No se encontraron  productos.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message; // Pasar error a la vista
                return View("WPS", null);
            }
        }

        [HttpGet("Production/WPS_Pallets")] 
        public async Task<ActionResult> Get_WPS_Pallets([FromQuery] int DocEntry)
        {
            try
            {
                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });


                AbxProductionOrders AbxProductionOrders = new AbxProductionOrders(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await AbxProductionOrders.Get_WPS_Pallets(userId, DocEntry);

                if (resultadoJson != null)
                {
                    string jsonString = JsonConvert.SerializeObject(resultadoJson);
                    Console.WriteLine($"Resultado JSON: {jsonString}");

                    if (jsonString != "")
                        return Ok(resultadoJson);
                    else
                        throw new Exception("No se encontro Pallet");
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    return NotFound("No se encontraron Pallets");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor.",
                    error = ex.Message
                });
            }


        }

        [HttpGet("Production/WPS_GroupPallets")]
        public async Task<ActionResult> WPS_GroupPallets([FromQuery] int DocEntry)
        {
            try
            {
                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });


                AbxProductionOrders AbxProductionOrders = new AbxProductionOrders(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await AbxProductionOrders.Get_WPS_GroupPallets(userId, DocEntry);

                if (resultadoJson != null)
                {
                    string jsonString = JsonConvert.SerializeObject(resultadoJson);
                    Console.WriteLine($"Resultado JSON: {jsonString}");

                    if (jsonString != "")
                        return Ok(resultadoJson);
                    else
                        throw new Exception("No se encontro Pallet");
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    return NotFound("No se encontraron Pallets");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor.",
                    error = ex.Message
                });
            }


        }

        [HttpPatch("Production/WPS_DeleteLine_Pallet")]
        public async Task<ActionResult> DelteOrUpdateStatus(int EntryOf, int DocEntry, int IdLine)
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
                    AbxProductionOrders abxProductionOrders = new AbxProductionOrders(_context);
                    // Convierte la instancia a JSON

                    var resultadoJson = await abxProductionOrders.Delete_WPS_Pallet_LineStatus(userId, EntryOf, DocEntry, IdLine, "D");

                    if (resultadoJson)
                    {
                        oResp.IsError = false;
                        oResp.Message = "Actualizado satisfactoriamente.";
                        oResp.JsonRsp = null;
                        oResp.EntryObject = DocEntry.ToString();
                        return Ok(oResp);
                        // oResp.EntryObject = sapResp.DocumentNumber.ToString();
                    }
                    else
                    {
                        throw new Exception("Error de actualizacion." + _context.Error);

                    }

                }
                catch (Exception ex)
                {
                    oResp.IsError = true;
                    oResp.Message = "Nose Actualizo el documento. " + ex.Message;
                    return BadRequest(oResp);
                }
                //return Ok(resultadoJson);

            }
            catch (Exception ex)
            {
                oResp.IsError = true;
                oResp.Message = $"Error al DelteOrUpdateStatus: {ex.Message}";
                return StatusCode(500, oResp);
            }

        }

        [HttpGet("Production/WPS_TimeStop")]
        public async Task<ActionResult> Get_WPS_TimeStop([FromQuery] int DocEntry)
        {
            try
            {
                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });

                AbxProductionOrders AbxProductionOrders = new AbxProductionOrders(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await AbxProductionOrders.Get_WPS_TimeStop(userId, DocEntry);

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
                    return NotFound("No se encontraron Tiempos");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message; // Pasar error a la vista
                return View("WPS", null);
            }
        }

        [HttpPost("Production/WPS_TimeStop")]
        public async Task<ActionResult> Add_WPS_TimeStop([FromBody] StopTimer stopTimer)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            try
            {
                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });

                AbxProductionOrders oStopTimer = new AbxProductionOrders(_context);

                // Convierte la instancia a JSON
                string json = JsonConvert.SerializeObject(stopTimer, Formatting.Indented);
                //Console.WriteLine(json);

                SapResponse oSapRs = null;

                var resultadoJson = await oStopTimer.Get_WPS_TimeStop_JSON(userId, json);

                if (resultadoJson != null)
                {
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    string jsonSAP = JsonConvert.SerializeObject(resultadoJson, Formatting.Indented);

                    var jsnVal = JsonConvert.DeserializeObject<Pallets>(jsonSAP);


                    if (jsnVal.DocEntry == 0)
                        oSapRs = await _productionOrderService.Create_StopTimerRequestsAsync(jsonSAP);
                    else
                        oSapRs = await _productionOrderService.Patch_StopTimerRequestsAsync(jsnVal.DocEntry, resultadoJson);


                    if (!oSapRs.IsError)
                    {
                        var sapResp = JsonConvert.DeserializeObject<Pallets>(oSapRs.JsonRsp.ToString());
                        // result.JsonRsp = sapResp;
                        oResp.IsError = false;

                        oResp.Message = "Tiempo agregado, exitosamente.";
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
                    throw new Exception("No se Recupero la estructura del tiempo Sql");

                }

            }
            catch (Exception ex)
            {
                oResp.IsError = true;
                // Maneja excepciones y devuelve un error interno del servidor
                oResp.Message = $"Error al Add_WPS_TimeStop: {ex.Message}";
                return StatusCode(500, oResp);
            }

        }

        [HttpPatch("Production/WPS_TimeStop")]
        public async Task<ActionResult> Update_WPS_TimeStop([FromBody] StopTimer stopTimer)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            try
            {
                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });


                AbxProductionOrders oStopTimer = new AbxProductionOrders(_context);

                //stopTimer.DocEntry = DocEntry;
                // Convierte la instancia a JSON
                string json = JsonConvert.SerializeObject(stopTimer, Formatting.Indented);
                //Console.WriteLine(json);

                SapResponse oSapRs = null;

                var resultadoJson = await oStopTimer.GetUpdate_WPS_TimeStop_JSON(userId, json);

                if (resultadoJson != null)
                {
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    string jsonSAP = JsonConvert.SerializeObject(resultadoJson, Formatting.Indented);

                    var jsnVal = JsonConvert.DeserializeObject<Pallets>(jsonSAP);
                

                    if (jsnVal.DocEntry == 0)
                        oSapRs = await _productionOrderService.Create_StopTimerRequestsAsync(jsonSAP);
                    else
                        oSapRs = await _productionOrderService.Patch_StopTimerRequestsAsync(jsnVal.DocEntry, resultadoJson);


                    if (!oSapRs.IsError)
                    {
                        var sapResp = JsonConvert.DeserializeObject<Pallets>(oSapRs.JsonRsp.ToString());
                        // result.JsonRsp = sapResp;
                        oResp.IsError = false;

                        oResp.Message = "Tiempo actualizado, exitosamente.";
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
                    throw new Exception("No se Recupero la estructura del tiempo Sql");

                }

            }
            catch (Exception ex)
            {
                oResp.IsError = true;
                // Maneja excepciones y devuelve un error interno del servidor
                oResp.Message = $"Error al Update_WPS_TimeStop: {ex.Message}";
                return StatusCode(500, oResp);
            }

        }



        [HttpPost("Production/WPS_Line_CreateOt")]
        public async Task<ActionResult> WPS_Line_CreateOt(int DocEntry, int IdLine)
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
                    AbxProductionOrders abxProductionOrders = new AbxProductionOrders(_context);
                    // Convierte la instancia a JSON

                    var resultadoJson = await abxProductionOrders.Update_WPS_TimeStop_LineStatus(userId, DocEntry, IdLine, "C");

                    if (resultadoJson)
                    {
                        oResp.IsError = false;
                        oResp.Message = "Actualizado satisfactoriamente.";
                        oResp.JsonRsp = null;
                        oResp.EntryObject = DocEntry.ToString();
                        return Ok(oResp);
                        // oResp.EntryObject = sapResp.DocumentNumber.ToString();
                    }
                    else
                    {
                        throw new Exception("Error de actualizacion." + _context.Error);

                    }

                }
                catch (Exception ex)
                {
                    oResp.IsError = true;
                    oResp.Message = "Nose Actualizo el documento. " + ex.Message;
                    return BadRequest(oResp);
                }
                //return Ok(resultadoJson);

            }
            catch (Exception ex)
            {
                oResp.IsError = true;
                oResp.Message = $"Error al DelteOrUpdateStatus: {ex.Message}";
                return StatusCode(500, oResp);
            }

        }


        [HttpPatch("Production/WPS_Line_UpdateStatus")]
        public async Task<ActionResult> DelteOrUpdateStatus(int DocEntry, int IdLine)
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
                    AbxProductionOrders abxProductionOrders = new AbxProductionOrders(_context);
                    // Convierte la instancia a JSON

                    var resultadoJson = await abxProductionOrders.Update_WPS_TimeStop_LineStatus(userId, DocEntry, IdLine, "C");

                    if (resultadoJson)
                    {
                        oResp.IsError = false;
                        oResp.Message = "Actualizado satisfactoriamente.";
                        oResp.JsonRsp = null;
                        oResp.EntryObject = DocEntry.ToString();
                        return Ok(oResp);
                        // oResp.EntryObject = sapResp.DocumentNumber.ToString();
                    }
                    else
                    {
                        throw new Exception("Error de actualizacion." + _context.Error);

                    }

                }
                catch (Exception ex)
                {
                    oResp.IsError = true;
                    oResp.Message = "Nose Actualizo el documento. " + ex.Message;
                    return BadRequest(oResp);
                }
                //return Ok(resultadoJson);

            }
            catch (Exception ex)
            {
                oResp.IsError = true;
                oResp.Message = $"Error al DelteOrUpdateStatus: {ex.Message}";
                return StatusCode(500, oResp);
            }

        }

        [HttpPatch("Production/WPS_UpdateStatusEnd")]
        public async Task<ActionResult> WPS_UpdateStatusEnd(string DocEntry)
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
                    AbxProductionOrders abxProductionOrders = new AbxProductionOrders(_context);
                    // Convierte la instancia a JSON

                    var resultadoJson = await abxProductionOrders.EndProductionOrder(userId, DocEntry);

                    if (resultadoJson)
                    {
                        oResp.IsError = false;
                        oResp.Message = "Finalizado satisfactoriamente.";
                        oResp.JsonRsp = null;
                        oResp.EntryObject = DocEntry.ToString();
                        return Ok(oResp);
                        // oResp.EntryObject = sapResp.DocumentNumber.ToString();
                    }
                    else
                    {
                        throw new Exception( _context.Error);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception( ex.Message);

                }
                //return Ok(resultadoJson);

            }
            catch (Exception ex)
            {
                oResp.IsError = true;
                oResp.Message = $"Error al Finalizado: {ex.Message}";
                return StatusCode(500, oResp);
            }

        }

        [HttpPost("Production/WPS_PrintPallet")]
        public async Task<ActionResult> WPS_PrintPallet(string DocEntry)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;

            // Usar IHttpClientFactory o HttpClient instanciado correctamente en la clase
            try
            {
                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });

                // Configuración de la solicitud HTTP
                try
                {
                    // Si _httpClient está inyectado, no es necesario crear una nueva instancia aquí
                    // Inyecta IHttpClientFactory en el constructor de la clase para instanciar HttpClient
                    HttpClient _httpClient = _httpClientFactory.CreateClient();  // Si usas IHttpClientFactory

                    AbxProductionOrders abxProductionOrders = new AbxProductionOrders(_context);

                    // Crear el cuerpo de la solicitud
                    var reportRequest = new
                    {
                        reportName = "003_Produccion",
                        parameters = new Dictionary<string, object>
                        {
                            ["DocKey@"] = DocEntry
                        }
                    };


                    // Crear contenido JSON para la solicitud POST
                    var content = new StringContent(JsonConvert.SerializeObject(reportRequest), Encoding.UTF8, "application/json");

                    // Realizar la solicitud POST al servicio externo
                    var response = await _httpClient.PostAsync(_context.Abi_UrlServerRPT + "CR_Production/GenerateReport", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Si la respuesta es exitosa, procesa el resultado
                        var contentType = response.Content.Headers.ContentType?.MediaType;

                        if (contentType == "application/pdf")
                        {
                            // Si la respuesta es un PDF, lo leemos como bytes
                            var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                            return File(pdfBytes, "application/pdf", "Reporte.pdf");
                        }
                        else
                        {
                            // Si la respuesta no es un PDF, la tratamos como JSON
                            var result = await response.Content.ReadAsStringAsync();
                            oResp.IsError = false;
                            oResp.Message = "Reporte generado satisfactoriamente.";
                            oResp.JsonRsp = result;
                            return Ok(oResp);
                        }
                    }
                    else
                    {
                        // Si la respuesta del servicio no es exitosa, maneja el error
                        oResp.Message = "Error al generar el reporte. Código de estado: " + response.StatusCode;
                        return StatusCode((int)response.StatusCode, oResp);
                    }
                }
                catch (Exception ex)
                {
                    oResp.Message = $"Error al generar el reporte: {ex.Message}";
                    return StatusCode(500, oResp);
                }
            }
            catch (Exception ex)
            {
                oResp.Message = $"Error general: {ex.Message}";
                return StatusCode(500, oResp);
            }
        }


        [HttpPost("Production/SolWork")]
        public async Task<ActionResult> Add_SolWork([FromBody] WorkOrder workOrder)
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

                AbxWorkOrders abxWorkOrders = new AbxWorkOrders(_context);

                // Convierte la instancia a JSON
                string json = JsonConvert.SerializeObject(workOrder, Formatting.Indented);
                //Console.WriteLine(json);

                SapResponse oSapRs = null;

                var resultadoJson = await abxWorkOrders.Add_Documents_SolicitudWork(userId, json);

                if (resultadoJson > 0)
                {
                    oResp.IsError = false;
                    oResp.Message = "Agregado satisfactoriamente.";
                    oResp.JsonRsp = null;
                    oResp.EntryObject = resultadoJson.ToString();
                    return Ok(oResp);
                    // oResp.EntryObject = sapResp.DocumentNumber.ToString();
                }
                else
                {
                    throw new Exception("Error al crear ." + _context.Error);
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
