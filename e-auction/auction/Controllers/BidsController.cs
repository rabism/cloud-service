using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using auction.Entity;
using auction.Exceptions;
using auction.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using auction.Commands;
using auction.Queries;
using auction.Services;
namespace auction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BidsController> _logger;
        public BidsController(ILogger<BidsController> logger, IMediator mediator
        )
        {
            _logger = logger;
            _mediator = mediator;

        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductBidDetail productBidDetail)
        {
            try
            {
                var product = await _mediator.Send(new ProductSearchByProductIdQuery()
                {
                    ProductId = productBidDetail.ProductId
                });

                if (product == null)
                {
                    throw new ProductNotFoundException($"Buyer with {productBidDetail.ProductId} not exist!!");
                }
                else if (DateTime.Now.Date >= product.BidEndDate.Date)
                {
                    throw new ProductBidExpiredException($"Can't palce bid after product bid end date ");
                }
                var buyer = await _mediator.Send(new BuyerDetailSearchByEmailQuery()
                {
                    BuyerEmail = productBidDetail.BuyerEmail
                });
                if (buyer == null)
                {
                    throw new BuyerNotFoundException($"Buyer with {productBidDetail.BuyerEmail} not registered!!");
                }

                var productBid = await _mediator.Send(new ProductBidSearchByProductIdSellerQuery()
                {
                    BuyerEmail = productBidDetail.BuyerEmail,
                    ProductId = productBidDetail.ProductId
                });

                if (productBid != null)
                {
                    throw new ProductBidExistException($"Buyer {productBidDetail.BuyerEmail} already bid this product {productBidDetail.ProductId}!!");
                }

                ProductBid mapProductBid = MapToEntity(productBidDetail, buyer, product);

                AddProductBidCommand command = new AddProductBidCommand
                {
                    ProductBid = mapProductBid
                };

                await _mediator.Send(command);
                return CreatedAtAction(nameof(Post), "Added successfully");
            }
            catch (BuyerNotFoundException ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ProductNotFoundException ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ProductBidExistException ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ProductBidExpiredException ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(500);
            }
        }

        private ProductBid MapToEntity(ProductBidDetail productDetail,
        UserDetail buyer, Product product
        )
        {
            return new ProductBid
            {
                BidAmount = productDetail.BidAmount,
                UserDetail = buyer,
                Product = new Product
                {
                    ProductId = product.ProductId,
                    DetailDescription = product.DetailDescription,
                    ShortDescription=product.ShortDescription,
                    Category=product.Category,
                    ProductName = product.ProductName,
                    StartingPrice = product.StartingPrice,
                    BidEndDate = product.BidEndDate

                }
            };

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("{productId}/{buyerEmail}/{bidAmount}")]
        public async Task<IActionResult> Put(string productId, string buyerEmail, decimal bidAmount)
        {
            try
            {
                var product = await _mediator.Send(new ProductSearchByProductIdQuery()
                {
                    ProductId = productId
                });

                if (product == null)
                {
                    throw new ProductNotFoundException($"Buyer with {productId} not exist!!");
                }
                else if (DateTime.Now.Date >= product.BidEndDate.Date)
                {
                    throw new ProductBidExpiredException($"Can't palce bid after product bid end date ");
                }


                var productBid = await _mediator.Send(new ProductBidSearchByProductIdSellerQuery()
                {
                    BuyerEmail = buyerEmail,
                    ProductId = productId
                });

                if (productBid == null)
                {
                    throw new ProductBidNotExistException($"Buyer {buyerEmail} not bid this product {productId}!!");
                }


                productBid.BidAmount = bidAmount;

                UpdateProductBidAmountCommand command = new UpdateProductBidAmountCommand
                {
                    ProductBid = productBid
                };
                await _mediator.Send(command);

                return Ok("Bid amount update successfully");
            }
            catch (ProductNotFoundException ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ProductBidExpiredException ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ProductBidNotExistException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in fetching data: {ex.Message}");
                return StatusCode(500);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{productId}")]
        public async Task<IActionResult> Get(string productId)
        {
            try
            {
                var product = await _mediator.Send(new ProductSearchByProductIdQuery()
                {
                    ProductId = productId
                });

                if (product == null)
                {
                    throw new ProductNotFoundException($"Product id : {productId} not found!!");
                }

                var productBidList = await _mediator.Send(new ProductBidSearchByProductIdQuery()
                {
                    ProductId = productId
                });

                return Ok(productBidList);

            }
            catch (ProductNotFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in fetching data: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}