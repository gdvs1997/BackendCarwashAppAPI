using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendCarwashApp.Dominio.Models.DTO;
using BackendCarwashApp.Logica.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendCarwashApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginLogica _logica;

        public LoginController(ILoginLogica logica)
        {
            _logica = logica;
        }

        [HttpPost("Crear")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model)
        {
            return await _logica.CreateUser(model);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> LoginUser([FromBody] LoginUser model)
        {
            return await _logica.Login(model);
        }
    }
}