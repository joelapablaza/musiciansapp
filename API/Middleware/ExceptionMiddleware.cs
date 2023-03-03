using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // 1. Ejecuta la función delegada proporcionada por el middleware siguiente en la cadena de middleware.
                await _next(context);
            }
            catch (Exception ex)
            {
                // 2. Registra el error utilizando el logger y su nivel de log correspondiente.
                _logger.LogError(ex, ex.Message);

                // 3. Establece el tipo de contenido de la respuesta HTTP a JSON.
                context.Response.ContentType = "application/json";

                // 4. Establece el código de estado de la respuesta HTTP a 500 (Error interno del servidor).
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // 5. Crea una instancia de la clase ApiException. Si la aplicación está en modo de desarrollo, se incluirá información adicional de error como el stack trace.
                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internal Server Error");

                // 6. Crea una instancia de JsonSerializerOptions y establece la política de nomenclatura en Camel Case.
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                // 7. Serializa la instancia de ApiException a una cadena JSON utilizando JsonSerializer.
                var json = JsonSerializer.Serialize(response, options);

                // 8. Escribe la cadena JSON como respuesta HTTP.
                await context.Response.WriteAsync(json);
            }
        }
    }
}