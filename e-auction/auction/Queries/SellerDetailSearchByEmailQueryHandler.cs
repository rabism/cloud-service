using MediatR;
using auction.Entity;
using auction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace auction.Queries
{
    public class SellerDetailSearchByEmailQuery : IRequest<UserDetail>
    {
        public string SellerEmail { get; set; }
       
        
    }

    public class SellerDetailSearchByEmailQueryHandler : IRequestHandler<SellerDetailSearchByEmailQuery, UserDetail>
        {
            private readonly IProductDbContext _context;
            public SellerDetailSearchByEmailQueryHandler(IProductDbContext context)
            {
                _context = context;
            }
            public async Task<UserDetail> Handle(SellerDetailSearchByEmailQuery query, CancellationToken cancellationToken)
            {
                return await _context.UserDetail.FindAsync(x => x.Email.Equals(query.SellerEmail) && x.UserType.Equals(AppConstant.Seller) ).Result.FirstOrDefaultAsync();
            }
        }
}
