using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auction.Exceptions
{
    
    public class ProductBidNotExistException : ApplicationException
    {
        public ProductBidNotExistException() { }
        public ProductBidNotExistException(string message) : base(message) { }
    }
    
}
