using MediatR;
using auction.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace auction.Commands
{
    
    public class AddUserDetailCommand : IRequest<UserDetail>
    {
        public UserDetail Seller { get; set; }

        
    }
    public class AddUserDetailCommandHandler : IRequestHandler<AddUserDetailCommand, UserDetail>
    {

        readonly ILogger<AddUserDetailCommandHandler> logger;
        private readonly IProductDbContext _context;
        public AddUserDetailCommandHandler(IProductDbContext context, ILogger<AddUserDetailCommandHandler> _logger)
        {
            _context = context;
            logger = _logger;
        }
        public async Task<UserDetail> Handle(AddUserDetailCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _context.UserDetail.InsertOneAsync(command.Seller);
                logger.LogInformation("seller added success!");
                return command.Seller;
            }
            catch (Exception ex)
            {
                logger.LogError("Eror occurs on adding compny" + ex.Message);
                throw ex;
            }

        }
    }
    
}
