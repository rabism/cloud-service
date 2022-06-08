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
    public class UpdateProductBidAmountCommand : IRequest<int>
    {
        public ProductBid ProductBid{ get; set; }

        
    }
    public class UpdateProductBidAmountCommandHandler : IRequestHandler<UpdateProductBidAmountCommand,int>
        {
            private readonly IProductDbContext _context;
            public UpdateProductBidAmountCommandHandler(IProductDbContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(UpdateProductBidAmountCommand command, CancellationToken cancellationToken)
            {
               //var productBid= _context.productBid.FindAsync(x=>x.Id.Equals(command.ProductBid.Id));
               //await  _context.ProductBid.update(command.ProductBid);
               //return _context.SaveChangeAsync();
               await _context.ProductBid.ReplaceOneAsync(x => x.Id == command.ProductBid.Id, command.ProductBid);
               return 0;
                
            }
        }
}
