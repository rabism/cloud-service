using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auction.Exceptions
{
    
    public class SellerNotFoundException : ApplicationException
    {
        public SellerNotFoundException() { }
        public SellerNotFoundException(string message) : base(message) { }
    }
    
}
