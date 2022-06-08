using MediatR;
using auction.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using auction.Models;
namespace auction.Queries
{
    public class ProductBidSearchByProductIdSellerQuery : IRequest<ProductBid>
    {
        public string ProductId { get; set; }
        public string BuyerEmail { get; set; }
    }

    public class ProductBidSearchByProductIdSellerQueryHandler : IRequestHandler<ProductBidSearchByProductIdSellerQuery, ProductBid>
    {
        private readonly IProductDbContext _context;
        public ProductBidSearchByProductIdSellerQueryHandler(IProductDbContext context)
        {
            _context = context;
        }
        public async Task<ProductBid> Handle(ProductBidSearchByProductIdSellerQuery query, CancellationToken cancellationToken)
        {
            return await _context.ProductBid.FindAsync(x => x.Product.ProductId.Equals(query.ProductId) 
            && x.UserDetail.Email.Equals(query.BuyerEmail)
            && x.UserDetail.UserType.Equals(AppConstant.Buyer)
            ).Result.FirstOrDefaultAsync();
        }
    }
}
