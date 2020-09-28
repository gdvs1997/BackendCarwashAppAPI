using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackendCarwashApp.PersistenciaDatos.Repository
{
    public interface IRepository
    {
        Task CreatedAsync<T>(T entity) where T : class;

        Task<ActionResult<IEnumerable<T>>> GetAll<T>() where T : class;

        Task<T> GetById<T>(Expression<Func<T, bool>> t) where T : class;
    }
}
