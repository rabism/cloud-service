using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auction.Exceptions
{
   
    public class ProductConstrainException : ApplicationException
    {
         public ProductConstrainException() { }
        public ProductConstrainException(string message) : base(message) { }
    }
}