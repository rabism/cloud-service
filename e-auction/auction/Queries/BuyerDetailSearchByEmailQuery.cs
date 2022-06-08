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
    public class BuyerDetailSearchByEmailQuery : IRequest<UserDetail>
    {
        public string BuyerEmail { get; set; }
    }

    public class BuyerDetailSearchByEmailQueryHandler : IRequestHandler<BuyerDetailSearchByEmailQuery, UserDetail>
        {
            private readonly IProductDbContext _context;
            public BuyerDetailSearchByEmailQueryHandler(IProductDbContext context)
            {
                _context = context;
            }
            public async Task<UserDetail> Handle(BuyerDetailSearchByEmailQuery query, CancellationToken cancellationToken)
            {
                return await _context.UserDetail.FindAsync(x => x.Email.Equals(query.BuyerEmail) && x.UserType.Equals(AppConstant.Buyer) ).Result.FirstOrDefaultAsync();
            }
        }
}
