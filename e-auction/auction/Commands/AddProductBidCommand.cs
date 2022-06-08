using MediatR;
using auction.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace auction.Commands
{
    public class AddProductBidCommand : IRequest<ProductBid>
    {
        public ProductBid ProductBid{ get; set; }

        
    }
    public class AddProductBidCommandHandler : IRequestHandler<AddProductBidCommand,ProductBid>
        {
            private readonly IProductDbContext _context;
            public AddProductBidCommandHandler(IProductDbContext context)
            {
                _context = context;
            }
            public async Task<ProductBid> Handle(AddProductBidCommand command, CancellationToken cancellationToken)
            {
               
               await  _context.ProductBid.InsertOneAsync(command.ProductBid);
               return command.ProductBid;
                
            }
        }
}
