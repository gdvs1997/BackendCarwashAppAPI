using System;
using System.Collections.Generic;
using System.Text;

namespace BackendCarwashApp.Dominio.Models.DTO
{
    public class UserToken
    {
        public Boolean Ok { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Message { get; set; }
    }
}
