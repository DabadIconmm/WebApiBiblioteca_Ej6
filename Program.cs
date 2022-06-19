using Ejercicio_Sesión_1;
using Ejercicio_Sesión_1.Filtros;
using Ejercicio_Sesión_1.Middlewares;
using Ejercicio_Sesión_1.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

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
//6.1
builder.Services.AddTransient<HashService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("webapi", x => { x.BaseAddress = new Uri("https://localhost:44381"); });

builder.Services.AddHostedService<TareaProgramadaService>();

//Configuration JwT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(
                     Encoding.UTF8.GetBytes(builder.Configuration["ClaveJWT"])),
                   ClockSkew = TimeSpan.Zero
               });
//6.3 - Configutation Swagger with tokens
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

});

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
