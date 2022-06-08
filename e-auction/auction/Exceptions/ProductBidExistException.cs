using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auction.Exceptions
{
    
    public class ProductBidExistException : ApplicationException
    {
        public ProductBidExistException() { }
        public ProductBidExistException(string message) : base(message) { }
    }
    
}
