using Abixe_Logic;
using Abixe_Models;
using Control_Piso.Models;
using ControlFloor.Hubs;
using ControlFloor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace ControlFloor.Controllers
{
    [Route("Basculas")]
    public class BasculasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MessageHub> _hubContext;
        public BasculasController(ApplicationDbContext context, IHubContext<MessageHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("RecibirPeso")]
        public async Task<IActionResult> RecibirPeso([FromBody] BasculaData pesoBascula)
        {
            if (pesoBascula == null)
            {
                return BadRequest("Datos inválidos");
            }

            try
            {
                // Guardar el peso en la base de datos
                AbxBasculas oConAbxBasculas = new AbxBasculas(_context);
                string json = JsonConvert.SerializeObject(pesoBascula, Formatting.Indented);
                var resultado = await oConAbxBasculas.GuardarPesoBasculaAsync("", json);

                if (!resultado)
                {
                    return StatusCode(500, "Error al guardar los datos");
                }

                // Enviar los datos al hub de SignalR
                //    await _hubContext.Clients.All.SendAsync("RecibirPesos", pesoBascula);
                // Notificar al hub sobre el nuevo peso
                await _hubContext.Clients.All.SendAsync("RecibirPesos", new List<BasculaData> { pesoBascula });

                return Ok("Peso recibido y enviado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("PesosPorBascula")]
        public async Task<ActionResult<IEnumerable<BasculaData>>> ObtenerPesosPorBascula(string? idBascula)
        {
            try
            {
                // Buscar todos los registros de la báscula por su Id
                AbxBasculas oConAbxBasculas = new AbxBasculas(_context);

                var registros = await oConAbxBasculas.Get_Basculas("", idBascula);

                if (registros == null)
                {
                    return NotFound("No se encontraron registros para esta báscula.");
                }

                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(registros)}");
                return Json(registros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al consultar los datos: {ex.Message}");
            }
        }


        [HttpGet("Impresoras")]
        public async Task<ActionResult<IEnumerable<BasculaData>>> ObtenerImpresoras(string? idImpresora)
        {
            try
            {
                // Buscar todos los registros de la báscula por su Id
                AbxBasculas oConAbxBasculas = new AbxBasculas(_context);

                var registros = await oConAbxBasculas.Get_Impresoras("", idImpresora);

                if (registros == null)
                {
                    return NotFound("No se encontraron registros para impresoras.");
                }


                Console.WriteLine($"Resultado JSON: {JsonConvert.SerializeObject(registros)}");
                return Json(registros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al consultar los datos: {ex.Message}");
            }
        }

    }
}
