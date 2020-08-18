using BackendCarwashApp.Dominio.Models;
using BackendCarwashApp.Dominio.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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

        public LoginLogica(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        public async Task<ActionResult<UserToken>> CreateUser(UserInfo model)
        {
            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return BuildToken(new LoginUser { UserName = model.UserName, Password = model.Password });
            }
            else
            {
                return new UserToken
                {
                    Ok = false,
                    Token = null,
                    Expiration = DateTime.Now,
                    Message = "Favor la contraseña debe contener por lo menos un caracter extraño y un numero."
                };
            }
        }

        public async Task<ActionResult<UserToken>> Login(LoginUser user)
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
                    Message = "Usuario o Contraseña Incorrecta."
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
    }
}
