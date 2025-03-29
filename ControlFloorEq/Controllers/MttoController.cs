using Abixe_Logic;
using Abixe_Models;
using Abixe_Models.DTOs;
using Abixe_SapServiceLayer;
using Control_Piso.Models;
using ControlFloor.Models;
using ControlFloor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;

namespace ControlFloor.Controllers
{

    [Authorize]
    public class MttoController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly SapServiceCall _sapServiceCall;

        public MttoController(ApplicationDbContext context, SapServiceCall sapServiceCall)
        {
            _context = context;
            _sapServiceCall = sapServiceCall;
        }


        //[HttpGet]
        //public async Task<ActionResult> MttoListWorkOrders()
        //{
        //    return View();
        //}


        [Route("Mtto/MttoListWorkOrders")]
        [HttpGet]
        public async Task<ActionResult> ListMttoWorkOrders()
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxWorkOrders abxWorkOrders = new AbxWorkOrders(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await abxWorkOrders.Get_Documents_SolicitudWork(userId);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return View("MttoListWorkOrders", resultadoJson); // Enviamos los datos a la vista
                                                                      // return Json(resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron órdenes de trabajo para el usuario.");
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devolvemos un error en JSON
                //  return Json(new { success = false, errorMessage = ex.Message });
                ViewBag.ErrorMessage = ex.Message; // Pasar error a la vista
                return View("MttoListWorkOrders", null);
            }


        }

        [HttpPost("Mtto/SolWork")]
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

        [HttpGet("Mtto/ServiceCall")]
        public async Task<ActionResult> MttoCallServices(int IdSolWor)
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

                AbxServiceCalls abxServiceCalls = new AbxServiceCalls(_context);

                var resultadoJson = await abxServiceCalls.Get_Documents_ActivityByCallService(userId, IdSolWor);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return View("MttoCallServices", resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    return NotFound("No se encontraron solicitudes de trabajo para el usuario.");
                }
            }
            catch (Exception ex)
            {
                // Maneja excepciones y devuelve un error interno del servidor
                return View("MttoCallServices", null);
            }
        }

        //[HttpPost("Mtto/ServiceCall")]
        //public async Task<ActionResult> Add_ServiceCall(int IdSolWor)
        //{
        //    ResponseAbx oResp = new ResponseAbx();
        //    oResp.IsError = true;

        //    try
        //    {
        //        var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
        //        if (userIdClaim == null)
        //        {
        //            return Json(new { lista = (object)null, success = false });
        //        }
        //        string userId = userIdClaim.Value;

        //        AbxServiceCalls abxServiceCalls = new AbxServiceCalls(_context);
        //        var resultadoJson = await abxServiceCalls.Get_Documents_ServiceCalls(userId, IdSolWor);


        //        if (resultadoJson != null)
        //        {
        //            // Imprimir en consola para depuración
        //            //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
        //            Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");

        //            try
        //            {
        //                bool esValido = FnsGenerics.EsJsonValido(JsonConvert.SerializeObject(resultadoJson));

        //                if (esValido)
        //                {
        //                    // var oSAp = await _productionOrderService.LoginAsync();
        //                    //SapProductionOrderService opro = new SapProductionOrderService(_sapServiceLayer);
        //                    oResp.IsError = false;
        //                    var result = await _sapServiceCall.Create_ServiceCallAsync(JsonConvert.SerializeObject(resultadoJson));

        //                    if (!result.IsError)
        //                    {
        //                        var sapResp = JsonConvert.DeserializeObject<DocumentsResponse>(result.JsonRsp.ToString());

        //                        oResp.JsonRsp = sapResp;
        //                        oResp.EntryObject = sapResp.DocNum.ToString();
        //                        oResp.Message = "Llamada de Servicio Creada satisfactoriamente, No. " + sapResp.DocNum.ToString();

        //                        return Ok(oResp);
        //                    }
        //                    else
        //                    { throw new Exception(result.Message); }
        //                }
        //                else
        //                {  // no es json 
        //                    throw new Exception("Error en recuperar el Json");

        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                oResp.IsError = true;
        //                oResp.Message = "Error a convertir enviar SL " + ex.Message;
        //                throw new Exception("Error a convertir enviar SL " + ex.Message);

        //            }
        //            //return Ok(resultadoJson);
        //        }
        //        else
        //        {
        //            throw new Exception("No se recupero estructura de Solcitud get Sql");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        oResp.IsError = true;
        //        // Maneja excepciones y devuelve un error interno del servidor
        //        oResp.Message = $"Error : {ex.Message}";
        //        return StatusCode(500, oResp);
        //    }

        //}

      
        [HttpPost("Mtto/ServiceCall")]
        public async Task<ActionResult> Add_ServiceCall(int IdSolWor)
        {
            var oResp = new ResponseAbx { IsError = true };

            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    oResp.Message = "Usuario no autenticado.";
                    return Unauthorized(oResp);
                }

                string userId = userIdClaim.Value;
                var abxServiceCalls = new AbxServiceCalls(_context);
                var resultadoJson = await abxServiceCalls.Get_Documents_ServiceCalls(userId, IdSolWor);

                if (resultadoJson == null)
                {
                    throw new Exception("No se recupero estructura de Solcitud get Sql");
                }

                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");

                if (!FnsGenerics.EsJsonValido(JsonConvert.SerializeObject(resultadoJson)))
                {
                    throw new Exception("Error en recuperar el Json");
                }

                var result = await _sapServiceCall.Create_ServiceCallAsync(JsonConvert.SerializeObject(resultadoJson));

                if (result.IsError)
                {
                    throw new Exception(result.Message);
                }

                var sapResp = JsonConvert.DeserializeObject<DocumentsResponse>(result.JsonRsp.ToString());

                oResp.IsError = false;
                oResp.JsonRsp = sapResp;
                oResp.EntryObject = sapResp.DocNum.ToString();
                oResp.Message = $"Llamada de Servicio Creada satisfactoriamente, No. {sapResp.DocNum}";

                var bResp = await abxServiceCalls.Update_Status_CloseWork(userId, IdSolWor.ToString());

                return Ok(oResp);
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

        [HttpGet("Mtto/Activities")]
        public async Task<ActionResult> Get_Activitys(int CallId)
        {
            var oResp = new ResponseAbx { IsError = true };
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized("Usuario no autenticado o claim inválido.");
                }
                string userId = userIdClaim.Value;

                var abxWorkOrders = new AbxWorkOrders(_context);
                var resultadoJson = await abxWorkOrders.Get_Activities(userId, CallId);

                if (resultadoJson == null)
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    return NotFound("No se encontraron actividades.");
                }

                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                return Ok(resultadoJson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las Actividades: {ex.Message}");
            }
        }

        [HttpPost("Mtto/Activities")]
        public async Task<ActionResult> Add_Activitys([FromBody] Activities activitie)
        {
            var oResp = new ResponseAbx { IsError = true };

            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized("Usuario no autenticado o claim inválido.");
                }

                string userId = userIdClaim.Value;
                var abxWorkOrders = new AbxWorkOrders(_context);

                string json = JsonConvert.SerializeObject(activitie, Formatting.Indented);
                var resultadoJson = await abxWorkOrders.Get_AddOrUpdate_Activities(userId, json, 0);

                if (resultadoJson == null)
                {
                    throw new Exception("No se recupero estructura de Solcitud get Sql");
                }

                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");

                if (!FnsGenerics.EsJsonValido(JsonConvert.SerializeObject(resultadoJson)))
                {
                    throw new Exception("Error en recuperar el Json");
                }

                var result = await _sapServiceCall.Create_ActivitiesCallAsync(JsonConvert.SerializeObject(resultadoJson));

                if (result.IsError)
                {
                    throw new Exception(result.Message);
                }

                var sapResp = JsonConvert.DeserializeObject<ActivitiesResponse>(result.JsonRsp.ToString());

                if (sapResp.ActivityCode > 0)
                {
                    var ServiceCalls = new ServiceCalls
                    {
                        ServiceCallID = (int)activitie.IdCall,
                        ServiceCallActivities = new List<ServiceCallActivity> { new ServiceCallActivity { ActivityCode = (int)sapResp.ActivityCode, U_RealTime = activitie.RealTime } }

                    };

                    var Patchresult = await _sapServiceCall.Patch_ServiceCallAsync((int)activitie.IdCall, ServiceCalls);

                }

                oResp.IsError = false;
                oResp.JsonRsp = sapResp;
                oResp.EntryObject = sapResp.ActivityCode.ToString();
                oResp.Message = $"Actividad Creada satisfactoriamente, No. {sapResp.ActivityCode}";

                return Ok(oResp);
            }
            catch (Exception ex)
            {
                oResp.IsError = true;
                oResp.Message = $"Error a convertir enviar SL {ex.Message}";
                return StatusCode(500, oResp);
            }
        }


        [HttpGet("Mtto/SolTrasnfer")]
        public async Task<ActionResult> Get_SolTrasnfer(int CallId)
        {
            var oResp = new ResponseAbx { IsError = true };
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized("Usuario no autenticado o claim inválido.");
                }
                string userId = userIdClaim.Value;

                var abxWorkOrders = new AbxWorkOrders(_context);
                var resultadoJson = await abxWorkOrders.Get_SolTrasnfer(userId, CallId);

                if (resultadoJson == null)
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    return NotFound("No se encontraron actividades.");
                }

                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                return Ok(resultadoJson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las Actividades: {ex.Message}");
            }
        }

        [HttpPost("Mtto/SolTrasnfer")]
        public async Task<ActionResult> Add_SolTrasnfer([FromBody] AddSolStockTransfer solStockTransfer)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized("Usuario no autenticado o claim inválido.");
                }
                string userId = userIdClaim.Value;

                AbxWorkOrders abxWorkOrders = new AbxWorkOrders(_context);

                // Convierte la instancia a JSON
                string json = JsonConvert.SerializeObject(solStockTransfer, Formatting.Indented);
                var resultadoJson = await abxWorkOrders.Get_Add_SolTrasnfer(userId, json);

                if (resultadoJson == null)
                {
                    return BadRequest(new { IsError = true, Message = "No se recuperó estructura de solicitud de traslado." });
                }

                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");

                bool esValido = FnsGenerics.EsJsonValido(JsonConvert.SerializeObject(resultadoJson));
                if (!esValido)
                {
                    return BadRequest(new { IsError = true, Message = "El JSON no tiene el formato correcto." });
                }

                oResp.IsError = false;
                var result = await _sapServiceCall.Create_SolTransferCallAsync(JsonConvert.SerializeObject(resultadoJson));

                if (result.IsError)
                {
                    throw new Exception(  result.Message );
                }

                var sapResp = JsonConvert.DeserializeObject<DocumentsResponse>(result.JsonRsp.ToString());
                oResp.JsonRsp = sapResp;
                oResp.EntryObject = sapResp.DocNum.ToString();
                oResp.Message = $"Solicitud de Traslado Creada satisfactoriamente, No. {sapResp.DocNum}";
                oResp.IsError = false;
                return Ok(oResp); throw new Exception("No se recupero estructura de Solcitud get Sql");
                
            }
            catch (Exception ex)
            {
                // Maneja excepciones y devuelve un error interno del servidor
                return StatusCode(500, new { IsError = true, Message = $"Error en el servidor: {ex.Message}" });
            }
        }




        [HttpGet("Mtto/PurchaseOrder")]
        public async Task<ActionResult> Get_PurchaseOrder(int CallId)
        {
            var oResp = new ResponseAbx { IsError = true };
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized("Usuario no autenticado o claim inválido.");
                }
                string userId = userIdClaim.Value;

                var abxWorkOrders = new AbxWorkOrders(_context);
                var resultadoJson = await abxWorkOrders.Get_SolPurchase(userId, CallId);

                if (resultadoJson == null)
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    return NotFound("No se encontraron actividades.");
                }

                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                return Ok(resultadoJson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las Actividades: {ex.Message}");
            }
        }

        [HttpPost("Mtto/PurchaseOrder")]
        public async Task<ActionResult> Add_PurchaseOrder([FromBody] DTOPurchaseOrder oPurchaseOrder)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            int xCallid = 0;
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized("Usuario no autenticado o claim inválido.");
                }
                string userId = userIdClaim.Value;

                if (oPurchaseOrder.DocumentLines == null || oPurchaseOrder.DocumentLines.Count == 0)
                {
                    throw new Exception("No se encontraron líneas de documento.");
                }

                oPurchaseOrder.DocDate = DateTime.Now;
                oPurchaseOrder.DocType = "dDocument_Items";
                oPurchaseOrder.IdUser = Convert.ToInt32(userId);

                xCallid = oPurchaseOrder.DocumentLines[0].U_BISIT_CallId;

                AbxWorkOrders abxWorkOrders = new AbxWorkOrders(_context);

                // Convierte la instancia a JSON
                string json = JsonConvert.SerializeObject(oPurchaseOrder, Formatting.Indented);
                //Console.WriteLine(json);
                var resultadoJson = await abxWorkOrders.Get_Add_SolPurchase(userId, json);


                if (resultadoJson != null)
                {

                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");


                    bool esValido = FnsGenerics.EsJsonValido(JsonConvert.SerializeObject(resultadoJson));

                    if (esValido)
                    {

                        oResp.IsError = false;
                        var result = await _sapServiceCall.Create_SolPurchaseCallAsync(JsonConvert.SerializeObject(resultadoJson));

                        var sapResp = JsonConvert.DeserializeObject<DocumentsResponse>(result.JsonRsp.ToString());

                        //if (sapResp.DocEntry > 0)
                        //{
                        //    var ServiceCalls = new ServiceCalls
                        //    {
                        //        ServiceCallID = xCallid,
                        //        ServiceCallInventoryExpenses = new List<ServiceCallInventoryExpenses> { new ServiceCallInventoryExpenses { DocEntry = (int)sapResp.DocEntry, DocumentType = "edt_PurchaseRequests" } }
                        //    };

                        //    var Patchresult = await _sapServiceCall.Patch_ServiceCallAsync(xCallid, ServiceCalls);

                        //}

                        if (!result.IsError)
                        {


                            oResp.JsonRsp = sapResp;
                            oResp.EntryObject = sapResp.DocNum.ToString();
                            oResp.Message = "Solicitud de Compra Creada satisfactoriamente, No. " + sapResp.DocNum.ToString();

                            return Ok(oResp);
                        }
                        else
                        { throw new Exception(result.Message); }

                    }
                    else
                    {
                        throw new Exception("No tine el formato correcto JsonSAP ");
                    }


                }
                else
                {
                    throw new Exception("No se recupero estructura de Solcitud get Sql");
                }




                //var resultadoJson = await abxWorkOrders.Get_Add_SolTrasnfer(userId, json);

                //if (resultadoJson != null)
                //{
                //    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");

                //    bool esValido = FnsGenerics.EsJsonValido(JsonConvert.SerializeObject(resultadoJson));

                //    if (esValido)
                //    {
                //        // var oSAp = await _productionOrderService.LoginAsync();
                //        //SapProductionOrderService opro = new SapProductionOrderService(_sapServiceLayer);
                //        oResp.IsError = false;
                //        var result = await _sapServiceCall.Create_SolTransferCallAsync(JsonConvert.SerializeObject(resultadoJson));

                //        if (!result.IsError)
                //        {
                //            var sapResp = JsonConvert.DeserializeObject<DocumentsResponse>(result.JsonRsp.ToString());

                //            oResp.JsonRsp = sapResp;
                //            oResp.EntryObject = sapResp.DocNum.ToString();
                //            oResp.Message = "Solicitud de Traslado Creada satisfactoriamente, No. " + sapResp.DocNum.ToString();

                //            return Ok(oResp);
                //        }
                //        else
                //        { throw new Exception(result.Message); }
                //    }
                //    else
                //    {
                //        throw new Exception("No tine el formato correcto JsonSAP ");
                //    }

                //}
                //else
                //{
                //    throw new Exception("No se recupero estructura de Solcitud get Sql");
                //}
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    error = "Error interno del servidor.",
                });
            }
        }



        [HttpGet("Mtto/PurchaseOrderExt")]
        public async Task<ActionResult> Get_PurchaseOrderExt(int CallId)
        {
            var oResp = new ResponseAbx { IsError = true };
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized("Usuario no autenticado o claim inválido.");
                }
                string userId = userIdClaim.Value;

                var abxWorkOrders = new AbxWorkOrders(_context);
                var resultadoJson = await abxWorkOrders.Get_SolPurchase(userId, CallId);

                if (resultadoJson == null)
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    return NotFound("No se encontraron actividades.");
                }

                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                return Ok(resultadoJson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las Actividades: {ex.Message}");
            }
        }

        [HttpPost("Mtto/PurchaseOrderExt")]
        public async Task<ActionResult> Add_PurchaseOrderExt([FromBody] DTOPurchaseOrder oPurchaseOrder)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            int xCallid = 0;
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized("Usuario no autenticado o claim inválido.");
                }
                string userId = userIdClaim.Value;

                if (oPurchaseOrder.DocumentLines == null || oPurchaseOrder.DocumentLines.Count == 0)
                {
                    throw new Exception("No se encontraron líneas de documento.");
                }

                oPurchaseOrder.DocDate = DateTime.Now;
                oPurchaseOrder.DocType = "dDocument_Items";
                oPurchaseOrder.IdUser = Convert.ToInt32(userId);

                xCallid = oPurchaseOrder.DocumentLines[0].U_BISIT_CallId;

                AbxWorkOrders abxWorkOrders = new AbxWorkOrders(_context);

                // Convierte la instancia a JSON
                string json = JsonConvert.SerializeObject(oPurchaseOrder, Formatting.Indented);
                //Console.WriteLine(json);
                var resultadoJson = await abxWorkOrders.Get_Add_SolPurchase(userId, json);


                if (resultadoJson != null)
                {

                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");


                    bool esValido = FnsGenerics.EsJsonValido(JsonConvert.SerializeObject(resultadoJson));

                    if (esValido)
                    {

                        oResp.IsError = false;
                        var result = await _sapServiceCall.Create_SolPurchaseCallAsync(JsonConvert.SerializeObject(resultadoJson));

                        var sapResp = JsonConvert.DeserializeObject<DocumentsResponse>(result.JsonRsp.ToString());

                        //if (sapResp.DocEntry > 0)
                        //{
                        //    var ServiceCalls = new ServiceCalls
                        //    {
                        //        ServiceCallID = xCallid,
                        //        ServiceCallInventoryExpenses = new List<ServiceCallInventoryExpenses> { new ServiceCallInventoryExpenses { DocEntry = (int)sapResp.DocEntry, DocumentType = "edt_PurchaseRequests" } }
                        //    };

                        //    var Patchresult = await _sapServiceCall.Patch_ServiceCallAsync(xCallid, ServiceCalls);

                        //}

                        if (!result.IsError)
                        {


                            oResp.JsonRsp = sapResp;
                            oResp.EntryObject = sapResp.DocNum.ToString();
                            oResp.Message = "Solicitud de Compra Creada satisfactoriamente, No. " + sapResp.DocNum.ToString();

                            return Ok(oResp);
                        }
                        else
                        { throw new Exception(result.Message); }

                    }
                    else
                    {
                        throw new Exception("No tine el formato correcto JsonSAP ");
                    }


                }
                else
                {
                    throw new Exception("No se recupero estructura de Solcitud get Sql");
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    error = "Error interno del servidor.",
                });
            }
        }

        [HttpPost("Mtto/AdjuntarEvidencias")]
        public async Task<IActionResult> AdjuntarEvidencias([FromForm] List<IFormFile> archivos,
                                                    [FromForm] List<IFormFile> imagenesAntes,
                                                    [FromForm] List<IFormFile> imagenesDespues,
                                                    [FromForm] int CallId)
        {
            try
            {
                string rutaBase = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(rutaBase))
                    Directory.CreateDirectory(rutaBase);

                List<string> archivosGuardados = new List<string>();

                async Task GuardarArchivo(List<IFormFile> lista, string carpeta)
                {
                    string rutaDestino = Path.Combine(rutaBase, carpeta);
                    if (!Directory.Exists(rutaDestino))
                        Directory.CreateDirectory(rutaDestino);

                    foreach (var archivo in lista)
                    {
                        string rutaArchivo = Path.Combine(rutaDestino, archivo.FileName);
                        using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                        {
                            await archivo.CopyToAsync(stream);
                        }
                        archivosGuardados.Add(rutaArchivo);
                    }
                }

                // Guardar los archivos en sus respectivas carpetas
                await GuardarArchivo(archivos, "ArchivosGenerales");
                await GuardarArchivo(imagenesAntes, "ImagenesAntes");
                await GuardarArchivo(imagenesDespues, "ImagenesDespues");

                // Crear el objeto de adjuntos
                var attachments = new SLAttachments
                {
                    Attachments2_Lines = new List<Attachments2_Lines>()
                };

                // Agregar los archivos generales
                foreach (var archivo in archivos)
                {
                    attachments.Attachments2_Lines.Add(new Attachments2_Lines
                    {
                        AttachmentDate = DateTime.Now,
                        SourcePath = Path.Combine(rutaBase, "ArchivosGenerales", archivo.FileName),
                        FileExtension = Path.GetExtension(archivo.FileName)?.TrimStart('.'),
                        FileName = Path.GetFileNameWithoutExtension(archivo.FileName),
                        Override = "tYES"
                    });
                }

                // Agregar las imágenes antes
                foreach (var archivo in imagenesAntes)
                {
                    attachments.Attachments2_Lines.Add(new Attachments2_Lines
                    {
                        AttachmentDate = DateTime.Now,
                        SourcePath = Path.Combine(rutaBase, "ImagenesAntes", archivo.FileName),
                        FileExtension = Path.GetExtension(archivo.FileName)?.TrimStart('.'),
                        FileName = Path.GetFileNameWithoutExtension(archivo.FileName),
                        Override = "tYES"
                    });
                }

                // Agregar las imágenes después
                foreach (var archivo in imagenesDespues)
                {
                    attachments.Attachments2_Lines.Add(new Attachments2_Lines
                    {
                        AttachmentDate = DateTime.Now,
                        SourcePath = Path.Combine(rutaBase, "ImagenesDespues", archivo.FileName),
                        FileExtension = Path.GetExtension(archivo.FileName)?.TrimStart('.'),
                        FileName = Path.GetFileNameWithoutExtension(archivo.FileName),
                        Override = "tYES"
                    });
                }

                // Serializar los adjuntos a JSON
                var jsonAttachments = JsonConvert.SerializeObject(attachments);

                // Crear los adjuntos en SAP o el sistema correspondiente
                var attachmentResult = await _sapServiceCall.CreateAttachmentsAsync(jsonAttachments);

                // Verificar si hubo un error al crear los adjuntos
                if (attachmentResult.IsError)
                    return BadRequest(new { success = false, message = "Error al adjuntar documentos: " + attachmentResult.Message });

                // Deserializar la respuesta de SAP para obtener la información del adjunto
                var sapRespAttch = JsonConvert.DeserializeObject<Attachments2>(attachmentResult.JsonRsp.ToString());

                // Crear la llamada de servicio con el AttachmentEntry de SAP
                var serviceCall = new ServiceCalls
                {
                    ServiceCallID = CallId,
                    AttachmentEntry = sapRespAttch.AbsoluteEntry
                };

                // Realizar la actualización del Service Call en SAP o el sistema correspondiente
                var patchResult = await _sapServiceCall.Patch_ServiceCallAsync(CallId, serviceCall);

                // Verificar si hubo un error al actualizar el Service Call
                if (patchResult.IsError)
                    return BadRequest(new { success = false, message = "Error al actualizar la llamada de servicio: " + patchResult.Message });

                return Ok(new { success = true, message = "Archivos subidos y llamada de servicio actualizada correctamente", archivosGuardados });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Error al subir archivos", error = ex.Message });
            }
        }


        [Route("Mtto/MttoPreventivo")]
        [HttpGet]
        public IActionResult MttoPreventivo()
        {
            return View();
        }

        [HttpPost("Mtto/ActivitiesPrevent")]
        public async Task<ActionResult> ActivitiesPrevent([FromBody] Activities activitie) ///  mandar CallId en 0
        {
            var oResp = new ResponseAbx { IsError = true };

            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized("Usuario no autenticado o claim inválido.");
                }

                string userId = userIdClaim.Value;
                var abxWorkOrders = new AbxWorkOrders(_context);

                string json = JsonConvert.SerializeObject(activitie, Formatting.Indented);
                var resultadoJson = await abxWorkOrders.Get_AddOrUpdate_Activities(userId, json, 0);

                if (resultadoJson == null)
                {
                    throw new Exception("No se recupero estructura de Solcitud get Sql");
                }

                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");

                if (!FnsGenerics.EsJsonValido(JsonConvert.SerializeObject(resultadoJson)))
                {
                    throw new Exception("Error en recuperar el Json");
                }

                var result = await _sapServiceCall.Create_ActivitiesCallAsync(JsonConvert.SerializeObject(resultadoJson));

                if (result.IsError)
                {
                    throw new Exception(result.Message);
                }

                var sapResp = JsonConvert.DeserializeObject<ActivitiesResponse>(result.JsonRsp.ToString());

                oResp.IsError = false;
                oResp.JsonRsp = sapResp;
                oResp.EntryObject = sapResp.ActivityCode.ToString();
                oResp.Message = $"Actividad Creada satisfactoriamente, No. {sapResp.ActivityCode}";

                return Ok(oResp);
            }
            catch (Exception ex)
            {
                oResp.IsError = true;
                oResp.Message = $"Error a convertir enviar SL {ex.Message}";
                return StatusCode(500, oResp);
            }
        }

        [HttpGet("Mtto/ListActivitiesPrevent")]
        public async Task<ActionResult> ActivitiesPrevent()
        {
            var oResp = new ResponseAbx { IsError = true };
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized("Usuario no autenticado o claim inválido.");
                }
                string userId = userIdClaim.Value;

                var abxWorkOrders = new AbxWorkOrders(_context);
                var resultadoJson = await abxWorkOrders.Get_ActivitiesPrevent(userId);



                if (resultadoJson == null)
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    return NotFound("No se encontraron actividades.");
                }

                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                return Ok(resultadoJson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las Actividades: {ex.Message}");
            }
        }







        [Route("Mtto/HRS")]
        [HttpGet]
        public IActionResult OpenHRSFile()
        {
            return View("HRS");


        }
        


    }



}

