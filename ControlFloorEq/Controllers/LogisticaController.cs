using Microsoft.AspNetCore.Mvc;

namespace ControlFloor.Controllers
{
	public class LogisticaController : Controller
	{
		// Acción para la vista principal de Logística
		public IActionResult Logistica()
		{
			return View(); // Esto renderizará la vista "Logistica.cshtml"
		}

		// Acción para la vista LogisticaOne
		public IActionResult LogisticaOne()
		{
			return View(); // Esto renderizará la vista "LogisticaOne.cshtml"
		}
	}
}