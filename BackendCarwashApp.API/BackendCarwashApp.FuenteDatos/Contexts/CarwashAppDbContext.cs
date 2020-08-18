using BackendCarwashApp.Dominio.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendCarwashApp.FuenteDatos.Contexts
{
    public class CarwashAppDbContext: IdentityDbContext<ApplicationUser>
    {
        public CarwashAppDbContext(DbContextOptions<CarwashAppDbContext> options) :base(options)
        {

        }
    }
}
