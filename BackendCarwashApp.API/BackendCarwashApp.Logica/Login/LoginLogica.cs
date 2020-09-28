using BackendCarwashApp.Dominio.Models;
using BackendCarwashApp.Dominio.Models.DTO;
using BackendCarwashApp.PersistenciaDatos.Repository;
using BackendCarwashApp.Transversal.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BackendCarwashApp.Logica.Login
{
    public class LoginLogica : ILoginLogica
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IRepository _repository;

        public LoginLogica(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IRepository repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _repository = repository;
        }
        public async Task<ActionResult<UserToken>> CreateUser(UserInfo model)
        {
            var validation = await ValidateUserInfo(model);
            if (validation.Codigo == HttpStatusCode.Found || validation.Codigo == HttpStatusCode.BadRequest)
            {
                return new UserToken
                {
                    Ok = false,
                    Message = validation.Mensaje
                };
            }
            else
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, PhoneNumber =  model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    
                    var BuscarUsuario = await _userManager.FindByNameAsync(model.UserName);
                    //Buscar usuario creado, mapearlo a la entidad usuario y crearlo en la tabla usuario

                    return BuildToken(new LoginUser { UserName = model.UserName, Password = model.Password });
                }
                else
                {
                    return new UserToken
                    {
                        Ok = false,
                        Message = "Error al crear el usuario."
                    };
                }
            }
        }

        public async Task<ActionResult<UserToken>> Login(LoginUser user)
        {
            var validateUser = await ValidateUserIfExist(user);
            if(validateUser.Codigo == HttpStatusCode.Found)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, user.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return BuildToken(user);
                }
                else
                {
                    //ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return new UserToken
                    {
                        Ok = false,
                        Message = "Contraseña Incorrecta."
                    };
                }
            }
            else
            {
                return new UserToken
                {
                    Ok = false,
                    Message = validateUser.Mensaje
                };
            }
        }

        private UserToken BuildToken(LoginUser userInfo)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Tiempo de expiracion del token, En nuestro caso lo hacemos de una hora.
            var expiration = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new UserToken()
            {
                Ok = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                Message = "Sesion Exitosa."
            };
        }

        private async Task<Request<bool>> ValidateUserInfo(UserInfo model)
        {
            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var _request = new Request<bool>();
            //Valida si el email existe
            var validateEmail = await _userManager.FindByEmailAsync(model.Email);
            //valida si el password cumple con lo requerido
            var validatePassword = await passwordValidator.ValidateAsync(_userManager, null, model.Password);
            //Valida si el Usuario existe
            var validateUsername = await _userManager.FindByNameAsync(model.UserName);

            if(validateUsername == null)
            {
                if(validateEmail == null)
                {
                    if (!validatePassword.Succeeded)
                    {
                        _request.Codigo = HttpStatusCode.BadRequest;
                        _request.Mensaje = "Su contraseña debe tener almenos una letra mayuscula, un numero y un caracter extraño.";
                    }
                    else
                    {
                        _request.Codigo = HttpStatusCode.Continue;
                    }
                }
                else
                {
                    _request.Codigo = HttpStatusCode.Found;
                    _request.Mensaje = "El correo que intenta registrar ya existe.";
                }
            }
            else
            {
                _request.Codigo = HttpStatusCode.Found;
                _request.Mensaje = "El usuario que intenta registrar ya existe.";
            }

            return _request;
        }

        private async Task<Request<bool>> ValidateUserIfExist(LoginUser model)
        {
            var _request = new Request<bool>();

            var validateUsername = await _userManager.FindByNameAsync(model.UserName);

            if (validateUsername == null)
            {
                _request.Codigo = HttpStatusCode.NotFound;
                _request.Mensaje = "El usuario no existe o esta incorrecto.";
            }
            else
            {
                _request.Codigo = HttpStatusCode.Found;
            }

            return _request;
        }
    }
}
