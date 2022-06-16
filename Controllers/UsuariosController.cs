using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ejercicio_Sesión_1.DTOs;
using Ejercicio_Sesión_1.Entidades;
using Ejercicio_Sesión_1.Services;

namespace Ejercicio_Sesión_1.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        private readonly HashService hashService;
        public UsuariosController(ApplicationDbContext context, IConfiguration configuration, HashService hashService)
        {
            this.context = context;
            this.configuration = configuration;
            this.hashService = hashService;
        }

        //6.2.b
        [HttpPost("hash/nuevousuario")]
        public async Task<ActionResult> PostNuevoUsuarioHash([FromBody] DTOUsuario usuario)
        {
            var resultadoHash = hashService.Hash(usuario.Password);
            var newUsuario = new Usuario
            {
                Email = usuario.Email,
                Password = resultadoHash.Hash,
                Salt = resultadoHash.Salt
            };

            await context.Usuarios.AddAsync(newUsuario);
            await context.SaveChangesAsync();

            return Ok(newUsuario);
        }

        //6.2.c
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] DTOUsuario usuario)
        {
            var usuarioDB = await context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (usuarioDB == null)
            {
                return BadRequest();
            }

            var resultadoHash = hashService.Hash(usuario.Password, usuarioDB.Salt);
            if (usuarioDB.Password == resultadoHash.Hash)
            {
                var response = GenerarToken(usuario);
                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }

        //6.2.d
        private DTOLoginResponse GenerarToken(DTOUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, credencialesUsuario.Email),
            };

            var clave = configuration["ClaveJWT"];
            var claveKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave));
            var signinCredentials = new SigningCredentials(claveKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new DTOLoginResponse()
            {
                Token = tokenString,
                Email = credencialesUsuario.Email
            };
        }
    }
}


