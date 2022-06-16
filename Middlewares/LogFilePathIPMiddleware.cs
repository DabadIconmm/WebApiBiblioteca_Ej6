namespace Ejercicio_Sesión_1.Middlewares
{
    public class LogFilePathIPMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<LogFilePathIPMiddleware> logger;

        public LogFilePathIPMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<LogFilePathIPMiddleware> logger)
        {
            this.next = next;
            this.env = env;
            this.logger = logger;
        }

        // Invoke o InvokeAsync
        public async Task InvokeAsync(HttpContext httpContext)
        {
            var IP = httpContext.Connection.RemoteIpAddress.ToString();
            //if (IP == "::1") // Bloquearía las peticiones de una IP
            //{
            //    httpContext.Response.StatusCode = 400;
            //    return;
            //}
            var ruta = httpContext.Request.Path.ToString();

            var path = $@"{env.ContentRootPath}\\wwwroot\log.txt";
            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                writer.WriteLine($@"{IP} - {ruta}");
                logger.LogInformation($@"{IP} - {ruta}");
            }

            await next(httpContext);
        }
    }
}
