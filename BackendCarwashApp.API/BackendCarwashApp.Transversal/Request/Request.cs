using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BackendCarwashApp.Transversal.Request
{
    public class Request<T>
    {
        public T Entity { get; set; }
        public String Mensaje { get; set; }
        public HttpStatusCode Codigo { get; set; }
        public String Exception { get; set; }
    }
}
