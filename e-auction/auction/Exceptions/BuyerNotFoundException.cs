using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auction.Exceptions
{
    
    public class BuyerNotFoundException : ApplicationException
    {
        public BuyerNotFoundException() { }
        public BuyerNotFoundException(string message) : base(message) { }
    }
    
}
