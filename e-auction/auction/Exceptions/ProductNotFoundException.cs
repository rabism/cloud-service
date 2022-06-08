using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auction.Exceptions
{
    
    public class ProductNotFoundException : ApplicationException
    {
        public ProductNotFoundException() { }
        public ProductNotFoundException(string message) : base(message) { }
    }
    
}
