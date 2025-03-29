using Abixe_Logic;
using Abixe_Models;
using Abixe_Models.DTOs;
using Abixe_SapServiceLayer;
using Control_Piso.Models;
using ControlFloor.Models;
using ControlFloor.Services;
using FastReport.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ControlFloor.Controllers
{
    [Authorize]
    public class PlanningController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SapProductionOrderService _productionOrderService;

        public PlanningController(ApplicationDbContext context, TokenService tokenService, SapProductionOrderService productionOrderService)
        {
            _context = context;
            _productionOrderService = productionOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> PlnEntrega()
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxSalesOrder SalesOrders = new AbxSalesOrder(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await SalesOrders.Get_Orders_detalils(userId, 0);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return View("PlnEntrega", resultadoJson); // Enviamos los datos a la vista
                    //return Json(resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron órdenes de venta para el usuario.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message; // Pasar error a la vista
                return View("PlnEntrega", null);
            }
        }


        [Route("Planning/ListarEntOrdenesVentaAsync")]
        [HttpGet]
        public async Task<JsonResult> ListarEntOrdenesVentaAsync()
        {

            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxSalesOrder SalesOrders = new AbxSalesOrder(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await SalesOrders.Get_Orders_detalils(userId, 0);

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
                    throw new Exception("No se encontraron órdenes de venta para el usuario.");
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devolvemos un error en JSON
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }

        [Route("Planning/MarcarOrdenEntregada")]
        [HttpPost]
        public async Task<ActionResult> MarcarOrdenEntregada(int DocEntry, int LineNum)
        {
            try
            {
                if (DocEntry <= 0 || LineNum < 0)
                {
                    return BadRequest(new { success = false, message = "Datos inválidos." });
                }

                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { success = false, message = "Usuario no autenticado." });
                }

                string userId = userIdClaim.Value;
                var salesOrders = new AbxSalesOrder(_context);

                var resultadoJson = await salesOrders.Update_Order_DeliberyOrProduction(userId, DocEntry, LineNum.ToString());

                if (resultadoJson == null)
                {
                    _context.Log($"Error: Update_Order_DeliberyOrProduction DocEntry={DocEntry}, LineNum={LineNum}");
                    return BadRequest(new { success = false, message = "Error al obtener estructura OF validar OV." });
                }

                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");

                if (!FnsGenerics.EsJsonValido(JsonConvert.SerializeObject(resultadoJson)))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Respuesta no válida.",
                        entryObject = DocEntry.ToString()
                    });
                }

                var result = await _productionOrderService.CreateProductionOrderAsync(JsonConvert.SerializeObject(resultadoJson));

                if (result.IsError)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Error en SAP: {result.Message}"
                    });
                }

                var sapResp = JsonConvert.DeserializeObject<ProductionOrderResponse>(result.JsonRsp.ToString());

                return Ok(new
                {
                    success = true,
                    message = $"Orden de fabricación creada exitosamente No. {sapResp.DocumentNumber}",
                    entryObject = sapResp.DocumentNumber
                });
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

        [HttpGet]
        public async Task<IActionResult> PlnListProgramProd()
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");/// Cargar las Ordenes de fabricacion
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxSalesOrder SalesOrders = new AbxSalesOrder(_context);
                // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                var resultadoJson = await SalesOrders.Get_Documents_ProductionOrders(userId, 0, 0);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return View("PlnListProgramProd", resultadoJson); // Enviamos los datos a la vista
                    //return Json(resultadoJson);
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron órdenes de producción para el usuario.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message; // Pasar error a la vista
                return View("PlnListProgramProd", null);
            }
        }

        //[Route("Planning/ListarPlnProgOrdenesProdAsync")]
        //[HttpGet]
        //public async Task<JsonResult> ListarPlnProgOrdenesProdAsync()
        //{
        //    try
        //    {
        //        var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");/// Cargar las Ordenes de fabricacion
        //        if (userIdClaim == null)
        //        {
        //            return Json(new { lista = (object)null, success = false });
        //        }
        //        string userId = userIdClaim.Value;

        //        AbxSalesOrder SalesOrders = new AbxSalesOrder(_context);
        //        // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
        //        var resultadoJson = await SalesOrders.Get_Documents_ProductionOrders(userId, 0, 0);

        //        if (resultadoJson != null)
        //        {
        //            // Imprimir en consola para depuración
        //            //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
        //            Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
        //            return Json(resultadoJson);
        //        }
        //        else
        //        {
        //            Console.WriteLine("El resultado es nulo o vacío.");
        //            throw new Exception("No se encontraron órdenes de producción para el usuario.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // En caso de error, devolvemos un error en JSON
        //        return Json(new { success = false, errorMessage = ex.Message });
        //    }

        //}



        [HttpPost("Planning/Update_Order_Delivery")]
        public async Task<ActionResult> Update_Order_Delivery(int DocEntry, string LineNum, string Status)
        {
            var response = new ResponseAbx { IsError = true };

            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { success = false, message = "Usuario no autenticado o claim inválido." });
                }

                string userId = userIdClaim.Value;
                var salesOrders = new AbxSalesOrder(_context);

                bool resultadoJson = await salesOrders.Update_Order_Status_Delivery(userId, DocEntry, LineNum, Status);

                if (!resultadoJson )
                {
                    throw new Exception("No se actualizó la orden de entrega.");
                }
                return Ok(new
                {
                    success = true,
                    message = "Orden enviada a Entrega.",
                    entryObject = DocEntry.ToString()
                });

                //response.IsError = false;
                //response.JsonRsp = resultadoJson;
                //response.EntryObject = DocEntry.ToString();
                //response.Message = "Orden enviada a Entrega.";

                //return Ok(response);
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


        #region Seccion para la venta na de Producción de planeación

        [HttpGet]
        public async Task<IActionResult> PlnProgramProd(int DocEntry)
        {
            try
            {
                string userId;

                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");
                    if (userIdClaim == null)
                    {
                        return View("PlnProgramProd", null); // Vista sin datos
                    }
                    userId = userIdClaim.Value;
                }
                else
                {
                    // En desarrollo, usar un IdUsuario de prueba
                    userId = "DevUser";
                }

                AbxSalesOrder SalesOrders = new AbxSalesOrder(_context);
                var resultadoJson = await SalesOrders.Get_Documents_ProductionOrders_Cabecera(userId, DocEntry);

                if (resultadoJson != null)
                {
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return View("PlnProgramProd", resultadoJson); // Enviamos los datos a la vista
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron órdenes de producción para el usuario.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message; // Pasar error a la vista
                return View("PlnProgramProd", null);
            }
        }

        [HttpGet("Planning/Get_Production_Orders_Lines")]
        public async Task<ActionResult> Get_Production_Orders_Lines([FromQuery] int DocEntry)
        {
            try
            {
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");/// Cargar las Ordenes de fabricacion
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxSalesOrder SalesOrders = new AbxSalesOrder(_context);

                var resultadoJson = await SalesOrders.Get_Documents_ProductionOrders_Cabecera(userId, DocEntry);

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
                    throw new Exception("No se encontraron órdenes de producción para el usuario.");
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devolvemos un error en JSON
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpPatch("Planning/Add_ComponentsOrUpdate")]
        public async Task<ActionResult> Add_ComponentsOrUpdate([FromBody] DTOMachine tOMachine, int DocEntry, int Status)
        {
            ResponseAbx oResp = new ResponseAbx { IsError = true };

            try
            {
                // 🔹 Obtención segura del UserId desde los Claims
                var identity = User.Identity as ClaimsIdentity;
                var userIdClaim = identity?.FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });
                }
                string userId = userIdClaim.Value;

                // 🔹 Validación de los parámetros recibidos
                if (tOMachine == null || DocEntry <= 0 || Status < 0)
                {
                    oResp.Message = "Datos inválidos: Verifique la componente, DocEntry o Status.";
                    return BadRequest(oResp);
                }

                // 🔹 Inicialización de objetos
                AbxProductionOrders abxProductionOrders = new AbxProductionOrders(_context);
                SapResponse oSapRs = null;

                try
                {
                    // 🔹 Convertir DTO a JSON y mostrar en consola para depuración
                    string json = JsonConvert.SerializeObject(tOMachine, Formatting.Indented);
                    Console.WriteLine($"📌 JSON enviado: {json}");

                    // 🔹 Llamar a la lógica para agregar el componente de producción
                    var resultadoJson = await abxProductionOrders.Add_Production_Componentes_Planeacion(userId, Status, DocEntry, json);

                    // Validar si se obtuvo una respuesta
                    if (resultadoJson == null)
                    {
                        oResp.Message = "No se recuperó la estructura de la máquina desde SQL.";
                        return BadRequest(oResp);
                    }

                    Console.WriteLine($"📌 Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");

                    // 🔹 Actualizar el estado del pedido de producción
                    oSapRs = await _productionOrderService.Update_Status_ProductionOrderAsync(DocEntry, resultadoJson);

                    //if (oSapRs == null)
                    //{
                    //    oResp.Message = "La respuesta de actualización de SAP es nula.";
                    //    return BadRequest(oResp);
                    //}

                    if (!oSapRs.IsError)
                    {
                        try
                        {
                            // 🔹 Verificar que `JsonRsp` no sea nulo antes de deserializar
                            //if (string.IsNullOrWhiteSpace(oSapRs.JsonRsp?.ToString()))
                            //{
                            //    throw new Exception("La respuesta de SAP está vacía o no es válida.");
                            //}

                            var sapResp = JsonConvert.DeserializeObject<ProductionOrderResponse>(oSapRs.JsonRsp.ToString());

                            // 🔹 Setear respuesta exitosa
                            oResp.IsError = false;
                            oResp.Message = Status == 0 ? "Máquina agregada exitosamente." : "Máquina modificada exitosamente.";
                            oResp.JsonRsp = sapResp;
                            oResp.EntryObject = DocEntry.ToString();

                            // 🔹 Si el status es 1, actualizar el estado de la línea de planeación
                            //if (Status == 1 && tOMachine?.LineNumReplace != null)
                            //{
                            //    var res = await abxProductionOrders.UPdate_Satatus_Line_Planeacion(userId, DocEntry, (int)tOMachine.LineNumReplace);
                            //    if (!res)
                            //    {
                            //        throw new Exception($" actualizar línea de planeación");
                            //    }
                            //}

                            //return Ok(oResp);
                            return Ok(new
                            {
                                success = true,
                                message = oResp.Message,
                                entryObject = DocEntry.ToString()
                            });
                        }
                        catch (JsonException jsonEx)
                        {
                            //Console.WriteLine($"❌ Error de deserialización JSON: {jsonEx.Message}");
                            //oResp.Message = "Error al procesar la respuesta de SAP.";
                            throw new Exception($"Error al procesar la respuesta de SAP: {oSapRs.Message}");
                            //return BadRequest(oResp);
                        }
                    }
                    else
                    {
                        // 🔹 Si hay error en la actualización de SAP
                        //oResp.Message = oSapRs.Message;
                        //oResp.JsonRsp = oSapRs.JsonRsp;
                        throw new Exception($"Error al procesar la máquina: {oSapRs.Message}");
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception($"Error al Componente la máquina: {ex.Message}");

                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    success = false,
                    message = "Error del servidor.",
                    error = $"Error al agregar o actualizar el componente: {ex.Message}"
                });
            }
        }

        [HttpPatch("Planning/PalletOrRollos")]
        public async Task<ActionResult> UPdate_Production_PalletOrRollos_Planeacion(int DocEntry, int Pallet, decimal Rollo, string Etiqueta)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            try
            {

                // 🔹 Obtención segura del UserId desde los Claims
                var identity = User.Identity as ClaimsIdentity;
                var userIdClaim = identity?.FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });
                }
                string userId = userIdClaim.Value;

                try
                {

                    AbxProductionOrders abxProductionOrders = new AbxProductionOrders(_context);
                    // Convierte la instancia a JSON

                    var resultadoJson = await abxProductionOrders.UPdate_Production_PalletOrRollos_Planeacion(userId, DocEntry, Pallet, Rollo, Etiqueta);

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

                return StatusCode(500, new
                {
                    success = false,
                    message = "Error del servidor.",
                    error = $"Error al agregar o actualizar el componente: {ex.Message}"
                });
            }
        }

        [HttpPatch("Planning/Add_MachineOrUpdate")]
        public async Task<ActionResult> Add_MachineOrUpdate_Planeacion([FromBody] DTOMachine tOMachine, int DocEntry, int Status)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            try
            {
                // 🔹 Obtención segura del UserId desde los Claims
                var identity = User.Identity as ClaimsIdentity;
                var userIdClaim = identity?.FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });
                }
                string userId = userIdClaim.Value;

                // 🔹 Validación de los parámetros recibidos
                if (tOMachine == null || DocEntry <= 0 || Status < 0)
                {
                    oResp.Message = "Datos inválidos: Verifique la componente, DocEntry o Status.";
                    return BadRequest(oResp);
                }

                SapResponse oSapRs = null;

                if (tOMachine != null)
                {
                    AbxProductionOrders abxProductionOrders = new AbxProductionOrders(_context);
                    // Convierte la instancia a JSON
                    string json = JsonConvert.SerializeObject(tOMachine, Formatting.Indented);

                    var resultadoJson = await abxProductionOrders.Add_Production_Componentes_Planeacion(userId, Status, DocEntry, json);

                    if (resultadoJson != null)
                    {
                        Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");

                        if (Status == 0)
                        {
                            oSapRs = await _productionOrderService.Update_Status_ProductionOrderAsync(DocEntry, resultadoJson);
                        }
                        else
                        {
                            var res = await abxProductionOrders.UPdate_Satatus_Line_Planeacion(userId, DocEntry, (int)tOMachine.LineNumReplace);
                            oSapRs = await _productionOrderService.Update_Status_ProductionOrderAsync(DocEntry, resultadoJson);
                        }

                        if (!oSapRs.IsError)
                        {
                            var sapResp = JsonConvert.DeserializeObject<ProductionOrderResponse>(oSapRs.JsonRsp.ToString());
                            // result.JsonRsp = sapResp;
                            oResp.IsError = false;
                            if (Status == 0)
                                oResp.Message = "Maquina agregada, exitosamente.";
                            else
                            {
                                oResp.Message = "Maquina modificada, exitosamente.";

                            }

                            return Ok(new
                            {
                                success = true,
                                message = oResp.Message,
                                entryObject = DocEntry.ToString()
                            });
                            // oResp.EntryObject = sapResp.DocumentNumber.ToString();
                        }

                        else
                        {
                            throw new Exception($"Error al procesar la respuesta de SAP: {oSapRs.Message}");

                        }
                    }
                    else
                    {
                        throw new Exception("No se Recupero la estructura de la maquina Sql");

                    }
                }
                else
                {
                    throw new Exception($"No cuenta con la estructura correcta");

                }

            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    success = false,
                    message = "Error del servidor.",
                    error = $"Error al agregar o actualizar el componente: {ex.Message}"
                });
            }

        }

        [HttpPatch("Planning/Status_OrderProduction")]
        public async Task<ActionResult> Update_Status_OrderProduction_Planeacion(int DocEntry, string Satus, string ItemCode)
        {
            ResponseAbx oResp = new ResponseAbx();
            oResp.IsError = true;
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                var userIdClaim = identity?.FindFirst("IdUsuario");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });
                }
                string userId = userIdClaim.Value;

                try
                {
                    //Satus (boposPlanned = Planeado , boposReleased = Liberado, ProductionOrderStatus)

                    string BISIT_Status = Satus;

                    //if (Satus == "boposReleased")
                    //    BISIT_Status = "R";
                    //else
                    //{
                    //    Satus = "boposReleased";
                    //    BISIT_Status = "L";
                    //}
                    switch (Satus)
                    {
                        case "P":
                            // BISIT_Status = "R";
                            Satus = "boposReleased";
                            break;
                        case "R":
                            // BISIT_Status = "L";
                            Satus = "boposReleased";
                            break;
                        case "L":
                            // BISIT_Status = "R";
                            Satus = "boposReleased";
                            break;
                    }

                    oResp.IsError = false;
                    var result = await _productionOrderService.Update_Status_ProductionOrderAsync(DocEntry, new { ProductionOrderStatus = Satus, ItemNo = ItemCode, U_BISIT_Status = BISIT_Status });


                    bool esValido = FnsGenerics.EsJsonValido(result.JsonRsp.ToString());

                    if (!result.IsError)
                    {
                        var sapResp = JsonConvert.DeserializeObject<ProductionOrderResponse>(result.JsonRsp.ToString());
                        // result.JsonRsp = sapResp;
                        oResp.JsonRsp = sapResp;
                        oResp.EntryObject = DocEntry.ToString();
                        return Ok(oResp);
                        // oResp.EntryObject = sapResp.DocumentNumber.ToString();
                    }
                    else
                    {
                        throw new Exception("Error a convertir enviar SL " + result.Message);
                    }


                }
                catch (Exception ex)
                {
                    throw new Exception("Error a convertir enviar SL " + ex.Message);

                }
                //return Ok(resultadoJson);

            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    success = false,
                    message = "Error del servidor.",
                    error = $"Error al agregar o actualizar Of: {ex.Message}"
                });
            }

        }


        [HttpPost("Planning/Add_DrafPurchaseRequest")]
        public async Task<ActionResult> Add_DrafPurchaseRequest([FromForm] AddPurchaseOrder addPurchaseOrder)
        {
            var response = new ResponseAbx { IsError = true };
            bool fileSaved = false;
            string attachmentPath = string.Empty;

            try
            {
                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });

                _context.Log(JsonConvert.SerializeObject(addPurchaseOrder, Formatting.Indented));
                var attachments = new SLAttachments();
                var anexosFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Anexos");
                Directory.CreateDirectory(anexosFolderPath); // Crea el directorio si no existe

                if (addPurchaseOrder.FileAttch?.Length > 0)
                {
                    var filePath = Path.Combine(anexosFolderPath, addPurchaseOrder.FileAttch.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await addPurchaseOrder.FileAttch.CopyToAsync(stream);

                    attachments.Attachments2_Lines = new List<Attachments2_Lines>
                    {
                        new Attachments2_Lines
                        {
                            AttachmentDate = DateTime.Now,
                            SourcePath = anexosFolderPath,
                            FileExtension = Path.GetExtension(addPurchaseOrder.FileAttch.FileName)?.TrimStart('.'),
                            FileName = Path.GetFileNameWithoutExtension(addPurchaseOrder.FileAttch.FileName),
                            Override = "tYES"
                        }
                    };
                    fileSaved = true;
                    attachmentPath = Path.Combine(anexosFolderPath, attachments.Attachments2_Lines[0].FileName + "." + attachments.Attachments2_Lines[0].FileExtension);
                }
            
                var UDo_Cab = new SapReq_U_BISIT_AUT_PRQ
                {
                    U_ReqType = "171",
                    U_Requester = userId,
                    U_AutStatus = "0",
                    U_AutPathFile = attachmentPath,
                    U_AutDate = null,
                    //U_CardCode = addPurchaseOrder.BISIT_AUT_PRQ1Collection.FindAll(x => x.U_CardCode != null).FirstOrDefault().U_CardCode,
                    BISIT_AUT_PRQ1Collection = addPurchaseOrder.BISIT_AUT_PRQ1Collection

                };

                //var purchaseOrder = new DTOPurchaseOrder
                //{
                //    CardCode = addPurchaseOrder.CardCode,
                //    DocDate = addPurchaseOrder.DocDate,
                //    DocEntry = addPurchaseOrder.DocEntry,
                //    LineNum = addPurchaseOrder.LineNum,
                //    Quantity = addPurchaseOrder.Quantity,
                //    PathAttachment = attachmentPath,
                //    Attachments = attachments
                //};

                var purchaseOrderService = new AbxPurchaseOrders(_context);
                //var purchaseRequestJson = await purchaseOrderService.Get_PurcharseRequest(userId, JsonConvert.SerializeObject(UDo_Cab, Formatting.Indented));

                if (UDo_Cab == null)
                    return Ok(new ResponseAbx { IsError = false, Message = "No se guardó la solicitud de compra." });

                Console.WriteLine("Resultado JSON: " + JsonConvert.SerializeObject(UDo_Cab));

                if (!FnsGenerics.EsJsonValido(JsonConvert.SerializeObject(UDo_Cab)))
                    return Ok(new ResponseAbx { IsError = false });

                var result = await _productionOrderService.CreateDraft_PurchaseRequestAsync(JsonConvert.SerializeObject(UDo_Cab));
                if (result.IsError)
                    throw new Exception(result.Message);

                var sapResponse = JsonConvert.DeserializeObject<SapReq_U_BISIT_AUT_PRQ>(result.JsonRsp.ToString());
                //response.IsError = false;
                //response.JsonRsp = sapResponse;
                //response.EntryObject = sapResponse.DocNum.ToString();
                //response.Message = $"Solicitud de Compra creada exitosamente No. {sapResponse.DocNum}";

                return Ok(new
                {
                    success = true,
                    message = $"Solicitud de Compra creada exitosamente No. {sapResponse.DocNum}",
                    entryObject = sapResponse.DocNum.ToString()
                });

            }
            catch (Exception ex)
            {
               // response.Message = 
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error en Add_DrafPurchaseRequest: {ex.Message}",
                error = ex.Message
                });
            }
        }


        #endregion


        [HttpGet]
        public async Task<IActionResult> PlnAutorizacion()
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

                AbxUser UserAut = new AbxUser(_context);
                var resultadoJson = await UserAut.Get_Auth_Documents(userId);


                if (resultadoJson != null)
                {
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return View("PlnAutorizacion", resultadoJson); // Enviamos los datos a la vista
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron órdenes de producción para el usuario.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message; // Pasar error a la vista
                return View("PlnProgramProd", null);
            }
        }

        [HttpGet("Planning/PlnAutorizacionLineas")]
        public async Task<IActionResult> PlnAutorizacionLineas(int Code)
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

                AbxUser UserAut = new AbxUser(_context);
                var resultadoJson = await UserAut.Get_Auth_DocumentsLineas(userId, Code.ToString());


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

        [HttpPost("Planning/DocumentsAuthorize")]
        public async Task<ActionResult> DocumentsAuthorize([FromBody] DTODocumentsAuthorize documentsAuthorize)
        {
            var oResp = new ResponseAbx { IsError = true };

            try
            {
                var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });

                var attachments = new SLAttachments();
                var purchaseOrders = new AbxPurchaseOrders(_context);

                if (documentsAuthorize.StatusAut == 1)
                {
                    if (!string.IsNullOrEmpty(documentsAuthorize.U_PathFile))
                    {
                        attachments.Attachments2_Lines = new List<Attachments2_Lines>
                        {
                            new Attachments2_Lines
                            {
                                AttachmentDate = DateTime.Now,
                                SourcePath = Path.GetDirectoryName(documentsAuthorize.U_PathFile),
                                FileExtension = Path.GetExtension(documentsAuthorize.U_PathFile)?.TrimStart('.'),
                                FileName = Path.GetFileNameWithoutExtension(documentsAuthorize.U_PathFile),
                                Override = "tYES"
                            }
                        };
                    }

                    var json = JsonConvert.SerializeObject(documentsAuthorize, Formatting.Indented);
                    var resultado = await purchaseOrders.Authorize_StatusDocuments(userId, json, documentsAuthorize.StatusAut);

                    if (resultado <= 0)
                        return BadRequest(new { success = false, message = "No se autorizó el documento." });

                    var jsonAttachments = JsonConvert.SerializeObject(attachments);
                    var attachmentResult = await _productionOrderService.CreateAttachmentsAsync(jsonAttachments);

                    if (attachmentResult.IsError)
                        return BadRequest(new { success = false, message = "Error al adjuntar documento: " + attachmentResult.Message });

                    var sapRespAttch = JsonConvert.DeserializeObject<Attachments2>(attachmentResult.JsonRsp.ToString());
                    documentsAuthorize.AbsoluteEntry = sapRespAttch.AbsoluteEntry;
                    documentsAuthorize.Attachments = attachments;
                    attachments.AbsoluteEntry = sapRespAttch.AbsoluteEntry;

                    json = JsonConvert.SerializeObject(documentsAuthorize, Formatting.Indented);
                    var resultadoSCJson = await purchaseOrders.Get_PurcharseOrder_JSON(userId, json);

                    if (resultadoSCJson == null)
                        throw new Exception("Sin estructura PurchaseRequests");

                    var result = await _productionOrderService.CreateDynamic_DocumentAsync(JsonConvert.SerializeObject(resultadoSCJson[0]), "PurchaseRequests");

                    if (result.IsError)
                        throw new Exception(result.Message);

                    var sapResp = JsonConvert.DeserializeObject<DocumentsResponse>(result.JsonRsp.ToString());
                    oResp.IsError = false;
                    oResp.JsonRsp = sapResp;
                    oResp.EntryObject = sapResp.DocNum.ToString();
                    oResp.Message = "Solicitud de Compra creada exitosamente No. " + sapResp.DocNum.ToString();

                    return Ok(oResp);
                }
                else
                {
                    var json = JsonConvert.SerializeObject(documentsAuthorize, Formatting.Indented);
                    var resultado = await purchaseOrders.Authorize_StatusDocuments(userId, json, documentsAuthorize.StatusAut);

                    if (resultado <= 0)
                        return BadRequest(new { success = false, message = "No se autorizó el documento." });

                    oResp.IsError = false;
                    oResp.EntryObject = documentsAuthorize.Code.ToString();
                    oResp.Message = "Documento rechazado exitosamente No. " + documentsAuthorize.Code.ToString();

                    return Ok(oResp);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, errorMessage = ex.Message });
            }
        }


        //[HttpPost("Planning/DocumentsAuthorize")]
        //public async Task<ActionResult> DocumentsAuthorize([FromBody] DTODocumentsAuthorize documentsAuthorize)
        //{
        //    ResponseAbx oResp = new ResponseAbx();
        //    oResp.IsError = true;
        //    try
        //    {

        //        var userId = User?.Identity is ClaimsIdentity identity ? identity.FindFirst("IdUsuario")?.Value : null;
        //        if (string.IsNullOrEmpty(userId))
        //            return Unauthorized(new { message = "Usuario no autenticado o claim inválido." });

        //        try
        //        {

        //            oResp.IsError = false;
        //            SLAttachments attachments = new SLAttachments();
        //            AbxPurchaseOrders PurchaseOrders = new AbxPurchaseOrders(_context);

        //            if (documentsAuthorize.StatusAut == 1)
        //            {

        //                //string filePath = documentsAuthorize.U_PathFile;

        //                //if (!string.IsNullOrEmpty(filePath))
        //                //{
        //                //    var att = new Attachments2_Lines()
        //                //    {
        //                //        AttachmentDate = DateTime.Now,
        //                //        SourcePath = Path.GetDirectoryName(filePath), // Obtiene el directorio base del archivo
        //                //        FileExtension = Path.GetExtension(filePath)?.TrimStart('.'),
        //                //        FileName = Path.GetFileNameWithoutExtension(filePath),
        //                //        Override = "tYES"
        //                //    };
        //                //    var listatt = new List<Attachments2_Lines>();
        //                //    listatt.Add(att);
        //                //    attachments.Attachments2_Lines = listatt;
        //                //}

        //                //string json = JsonConvert.SerializeObject(documentsAuthorize, Formatting.Indented);

        //                int resultado = await PurchaseOrders.Authorize_StatusDocuments(userId, json, documentsAuthorize.StatusAut);

        //                if (resultado > 0)
        //                {
        //                    oResp.IsError = false;

        //                    string jsonAttachments = JsonConvert.SerializeObject(attachments);
        //                    var attachmentResult = await _productionOrderService.CreateAttachmentsAsync(jsonAttachments);

        //                    if (!attachmentResult.IsError)
        //                    {
        //                        var sapRespAttch = JsonConvert.DeserializeObject<Attachments2>(attachmentResult.JsonRsp.ToString());

        //                        documentsAuthorize.AbsoluteEntry = sapRespAttch.AbsoluteEntry;
        //                        documentsAuthorize.Attachments = attachments;
        //                        attachments.AbsoluteEntry = sapRespAttch.AbsoluteEntry;
        //                        json = JsonConvert.SerializeObject(documentsAuthorize, Formatting.Indented);

        //                        var resultadoSCJson = await PurchaseOrders.Get_PurcharseOrder_JSON(userId, json);

        //                        if (resultadoSCJson != null)
        //                        {
        //                            var result = await _productionOrderService.CreateDynamic_DocumentAsync(JsonConvert.SerializeObject(resultadoSCJson[0]), "PurchaseRequests");

        //                            if (!result.IsError)
        //                            {
        //                                var sapResp = JsonConvert.DeserializeObject<DocumentsResponse>(result.JsonRsp.ToString());

        //                                oResp.JsonRsp = sapResp;
        //                                oResp.EntryObject = sapResp.DocNum.ToString();
        //                                oResp.Message = "Solicitud de Compra creada exitosamente No. " + sapResp.DocNum.ToString();


        //                                return Ok(oResp);
        //                            }
        //                            else
        //                            {
        //                                throw new Exception(result.Message);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            throw new Exception("Sin estructura PurchaseRequests ");
        //                        }


        //                    }
        //                    else
        //                    {
        //                        oResp.IsError = true;
        //                        oResp.Message = "Adjuntar documento.  " + attachmentResult.Message;

        //                        // Console.WriteLine("El resultado es nulo o vacío.");
        //                        return BadRequest(oResp);
        //                    }

        //                }
        //                else
        //                {
        //                    oResp.IsError = true;
        //                    oResp.Message = "Nose Autorizo el documento. ";

        //                    // Console.WriteLine("El resultado es nulo o vacío.");
        //                    return BadRequest(oResp);
        //                }

        //            }
        //            else
        //            {
        //                string json = JsonConvert.SerializeObject(documentsAuthorize, Formatting.Indented);

        //                int resultado = await PurchaseOrders.Authorize_StatusDocuments(userId, json, documentsAuthorize.StatusAut);

        //                if (resultado > 0)
        //                {
        //                    oResp.IsError = false;
        //                    oResp.EntryObject = documentsAuthorize.Code.ToString();
        //                    oResp.Message = "Documento rechazado exitosamente No. " + documentsAuthorize.Code.ToString();
        //                    return Ok(oResp);
        //                }
        //                else
        //                {
        //                    oResp.IsError = true;
        //                    oResp.Message = "Nose Autorizo el documento. ";

        //                    // Console.WriteLine("El resultado es nulo o vacío.");
        //                    return BadRequest(oResp);
        //                }
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            oResp.IsError = true;
        //            oResp.Message = "Nose Autorizo el documento. " + ex.Message;
        //            return BadRequest(oResp);
        //        }
        //        //return Ok(resultadoJson);

        //    }
        //    catch (Exception ex)
        //    {
        //        oResp.IsError = true;
        //        oResp.Message = $"Error al DocumentsAuthorize: {ex.Message}";
        //        return StatusCode(500, oResp);
        //    }

        //}


        [HttpGet("Planning/PlnSolCompra")]
        public async Task<ActionResult> PlnSolCompra([FromQuery] int DocNum)
        {
            try
            {

                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");/// Cargar las Ordenes de fabricacion
                if (userIdClaim == null)
                {
                    return Json(new { lista = (object)null, success = false });
                }
                string userId = userIdClaim.Value;

                AbxSalesOrder SalesOrders = new AbxSalesOrder(_context);

                var resultadoJson = await SalesOrders.Get_DocumentsListSolC_ProductionOrders_Cabecera(userId, DocNum);

                if (resultadoJson != null)
                {
                    // Imprimir en consola para depuración
                    //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
                    Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
                    return View("PlnSolCompra", resultadoJson); // Enviamos los datos a la vista
                }
                else
                {
                    Console.WriteLine("El resultado es nulo o vacío.");
                    throw new Exception("No se encontraron órdenes de producción para el usuario.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message; // Pasar error a la vista
                return View("PlnSolCompra", null);
            }
        }


        //[Route("Planning/ListarPlnProgOrdenesProdAsync")]
        //[HttpGet]
        //public async Task<JsonResult> ListarPlnMisComprasAsync()
        //{
        //    try
        //    {
        //        var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("IdUsuario");/// Cargar las Ordenes de fabricacion
        //        if (userIdClaim == null)
        //        {
        //            return Json(new { lista = (object)null, success = false });
        //        }
        //        string userId = userIdClaim.Value;

        //        AbxSalesOrder SalesOrders = new AbxSalesOrder(_context);
        //        // var xid = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
        //        var resultadoJson = await SalesOrders.Get_Documents_ProductionOrders(userId, 0, 0);

        //        if (resultadoJson != null)
        //        {
        //            // Imprimir en consola para depuración
        //            //JObject jobject = JObject.Parse(JsonConvert.SerializeObject(resultadoJson));
        //            Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(resultadoJson)}");
        //            return Json(resultadoJson);
        //        }
        //        else
        //        {
        //            Console.WriteLine("El resultado es nulo o vacío.");
        //            throw new Exception("No se encontraron órdenes de producción para el usuario.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // En caso de error, devolvemos un error en JSON
        //        return Json(new { success = false, errorMessage = ex.Message });
        //    }

        //}



    }
}
