namespace ControlFloor.Middleware
{
    public class GlobalLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalLoggingMiddleware> _logger;

        public GlobalLoggingMiddleware(RequestDelegate next, ILogger<GlobalLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Log de la solicitud
            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            _logger.LogInformation("Request Path: {Path}, Method: {Method}, Headers: {Headers}, Body: {Body}",
                                   context.Request.Path, context.Request.Method, context.Request.Headers, requestBody);
            context.Request.Body.Position = 0;

            // Captura la respuesta
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Procesa la solicitud
            await _next(context);

            // Log de la respuesta
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            _logger.LogInformation("Response Status: {StatusCode}, Headers: {Headers}, Body: {Body}",
                                   context.Response.StatusCode, context.Response.Headers, responseText);
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // Copia la respuesta al flujo original
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

}
