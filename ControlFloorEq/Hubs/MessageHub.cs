using Abixe_Logic;
using Control_Piso.Models;
using Microsoft.AspNetCore.SignalR;

namespace ControlFloor.Hubs
{
    public class MessageHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public MessageHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SendMessage(string deviceName, string message)
        {
            // Enviar mensaje a todos los clientes conectados
            await Clients.All.SendAsync("ReceiveMessage", deviceName, message);
        }

        public async Task ObtenerPesosPorBascula(string? idBascula)
        {
            try
            {
                // Consultar los registros desde la base de datos
                AbxBasculas basculaService = new AbxBasculas(_context);
                var registros = await basculaService.Get_Basculas("", idBascula);

                if (registros == null)
                {
                    // Notificar al cliente que no hay registros
                    await Clients.Caller.SendAsync("NoHayRegistros", $"No se encontraron registros para la báscula: {idBascula ?? "todas"}.");
                    return;
                }

                // Enviar los datos al cliente que invocó este método
                await Clients.All.SendAsync("RecibirPesos", registros);
                Console.WriteLine($"Notificación enviada: {idBascula} ");
            }
            catch (Exception ex)
            {
                // Notificar al cliente del error
                await Clients.Caller.SendAsync("Error", $"Error al obtener los datos: {ex.Message}");
            }
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Cliente conectado: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

    }
}
