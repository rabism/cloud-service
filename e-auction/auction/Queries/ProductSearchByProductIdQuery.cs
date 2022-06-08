using MediatR;
using auction.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace auction.Queries
{
    public class ProductSearchByProductIdQuery : IRequest<Product>
    {
        public string ProductId { get; set; }
    }

    public class ProductSearchByProductIdHandler : IRequestHandler<ProductSearchByProductIdQuery, Product>
        {
            private readonly IProductDbContext _context;
            public ProductSearchByProductIdHandler(IProductDbContext context)
            {
                _context = context;
            }
            public async Task<Product> Handle(ProductSearchByProductIdQuery query, CancellationToken cancellationToken)
            {
                return await _context.Product.FindAsync(x => x.ProductId.Equals(query.ProductId) ).Result.FirstOrDefaultAsync();
            }
        }
}
