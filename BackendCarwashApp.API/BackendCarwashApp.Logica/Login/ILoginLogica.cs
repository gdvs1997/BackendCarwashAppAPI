using BackendCarwashApp.Dominio.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BackendCarwashApp.Logica.Login
{
    public interface ILoginLogica
    {
        Task<ActionResult<UserToken>> CreateUser(UserInfo model);
        Task<ActionResult<UserToken>> Login(LoginUser user);
    }
}
