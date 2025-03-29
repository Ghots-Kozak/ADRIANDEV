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
	}
}