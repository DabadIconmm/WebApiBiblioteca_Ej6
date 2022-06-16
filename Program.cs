using Ejercicio_Sesión_1;
using Ejercicio_Sesión_1.Filtros;
using Ejercicio_Sesión_1.Middlewares;
using Ejercicio_Sesión_1.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Para evitar ciclos infinitos en entidades relacionadas
builder.Services.AddControllers(
    opciones => opciones.Filters.Add(typeof(FiltroDeExcepcion)) //para leer los filtros
    ).AddJsonOptions(opciones => opciones.JsonSerializerOptions.ReferenceHandler =
System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

//CORS
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(builder =>
    {
        //builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader();
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("defaultConnection"); //nos referimos a la conexion con appsetting.Development.json
// Registramos en el sistema de inyección de dependencias de la aplicación el ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
{
    opciones.UseSqlServer(connectionString);
    //Con esto hacemos que no se realize el traking de los registros de una BBDD, así somos explícitos donde queremos hacer el tracking.
    opciones.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddDataProtection();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("webapi", x => { x.BaseAddress = new Uri("https://localhost:44381"); });

builder.Services.AddHostedService<TareaProgramadaService>();

// Serilog
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

//Midlewares
app.UseMiddleware<LogFilePathIPMiddleware>();
app.UseMiddleware<LogFileBodyHttpResponseMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();//necesario para que funcione el CORS de arriba

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
