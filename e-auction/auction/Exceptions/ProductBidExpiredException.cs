using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auction.Exceptions
{
    
    public class ProductBidExpiredException : ApplicationException
    {
        public ProductBidExpiredException() { }
        public ProductBidExpiredException(string message) : base(message) { }
    }
    
}
