using Microsoft.AspNetCore.Mvc;

namespace ControlFloor.Controllers
{
	public class AlmacenController : Controller
	{
		// Acción para la vista principal de Almacén
		public IActionResult Almacen()
		{
			return View(); // Esto renderizará la vista "Almacen.cshtml"
		}
        

        public IActionResult Index(int solicitudId, string fecha, string documento, string solicitante, string almacenOrigen, string almacenDestino, string departamento, string estado)
        {
            var model = new SolicitudViewModel
            {
                SolicitudId = solicitudId,
                Fecha = fecha,
                Documento = documento,
                Solicitante = solicitante,
                AlmacenOrigen = almacenOrigen,
                AlmacenDestino = almacenDestino,
                Departamento = departamento,
                Estado = estado
            };
            return View(model);
        }


    }
}