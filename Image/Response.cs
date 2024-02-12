using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Domain
{
    public class Response
    {
        public bool Flag { get; set; }
        public string Message { get; set; }
        public object Object { get; set; }  
        public object Object1 { get; set; }
    }
}
