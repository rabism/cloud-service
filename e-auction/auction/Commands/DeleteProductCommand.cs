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
    public class DeleteProductCommand : IRequest<bool>
    {
        public string ProductId{get;set;}

        
    }
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand,bool>
        {
            private readonly IProductDbContext _context;
            private readonly ILogger<DeleteProductCommandHandler> logger;
            public DeleteProductCommandHandler(IProductDbContext context,ILogger<DeleteProductCommandHandler> _logger)
            {
                _context = context;
                logger=_logger;
            }
            public async Task<bool> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
            {
               try
               {
                await _context.Product.DeleteManyAsync(x => x.ProductId.Equals(command.ProductId));
               // await _context.Company.DeleteManyAsync(x => x.CompanyCode.Equals(command.companyCode));
                logger.LogInformation("Successfully delete the record");
                return true;
               }
               catch(Exception ex)
               {
                   logger.LogError("Error occurs on delete stock "+ex.Message);
                   return false;
               }
              
            }
        }
}
