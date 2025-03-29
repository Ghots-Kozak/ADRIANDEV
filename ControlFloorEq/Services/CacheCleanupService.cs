using Microsoft.Extensions.Caching.Memory;

namespace ControlFloor.Services
{
    public class CacheCleanupService : IHostedService, IDisposable
    {
        private readonly IMemoryCache _cache;
        private Timer _timer;

        public CacheCleanupService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Ejecutar limpieza cada 30 minutos
            _timer = new Timer(CleanUpCache, null, TimeSpan.Zero, TimeSpan.FromMinutes(90));
            return Task.CompletedTask;
        }

        private void CleanUpCache(object state)
        {
            // No se necesita limpiar manualmente ya que MemoryCache lo maneja,
            // pero puedes hacer otras limpiezas aquí si fuera necesario.
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
