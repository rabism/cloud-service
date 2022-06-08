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
    public class AddProductCommand : IRequest<Product>
    {
        public Product Product{ get; set; }

        
    }
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand,Product>
        {
            private readonly IProductDbContext _context;
            public AddProductCommandHandler(IProductDbContext context)
            {
                _context = context;
            }
            public async Task<Product> Handle(AddProductCommand command, CancellationToken cancellationToken)
            {
               await  _context.Product.InsertOneAsync(command.Product);
               return command.Product;
                
            }
        }
}
