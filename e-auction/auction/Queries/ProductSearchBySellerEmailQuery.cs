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
    public class ProductSearchBySellerEmailQuery : IRequest<IReadOnlyCollection<Product>>
    {
        public string SellerEmail { get; set; }
    }

    public class ProductSearchBySellerEmailHandler : IRequestHandler<ProductSearchBySellerEmailQuery, IReadOnlyCollection<Product>>
        {
            private readonly IProductDbContext _context;
            public ProductSearchBySellerEmailHandler(IProductDbContext context)
            {
                _context = context;
            }
            public async Task<IReadOnlyCollection<Product>> Handle(ProductSearchBySellerEmailQuery query, CancellationToken cancellationToken)
            {
                return await _context.Product.FindAsync(x => x.UserDetail.Email.Equals(query.SellerEmail)).Result.ToListAsync();
            }
        }
}
