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
    public class ProductBidSearchByProductIdQuery : IRequest<IReadOnlyList<ProductBid>>
    {
        public string ProductId { get; set; }
    }

    public class ProductBidSearchByProductIdQueryHandler : IRequestHandler<ProductBidSearchByProductIdQuery, IReadOnlyList<ProductBid>>
        {
            private readonly IProductDbContext _context;
            public ProductBidSearchByProductIdQueryHandler(IProductDbContext context)
            {
                _context = context;
            }
            public async Task<IReadOnlyList<ProductBid>> Handle(ProductBidSearchByProductIdQuery query, CancellationToken cancellationToken)
            {
                return await _context.ProductBid.FindAsync(x => x.Product.ProductId.Equals(query.ProductId) ).Result.ToListAsync();
            }
        }
}
