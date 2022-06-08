using System;
using System.Threading;
using auction.Commands;
using auction.Controllers;
using auction.Entity;
using auction.Models;
using auction.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace auction.test
{
    public class BidsControllerTest
    {
        BidsController bidsController;
        Mock<IMediator> _mediator;
        Mock<ILogger<BidsController>> _logger;
        UserDetail buyerDetails;
        UserDetail sellerDetails;
        Product productEntity;
        ProductBid productBid;
        ProductBidDetail request;
        public BidsControllerTest()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<BidsController>>();
            setUpMock();
            bidsController = new BidsController(_logger.Object, _mediator.Object);
        }
        void setUpMock()
        {

            request = new ProductBidDetail
            {
                ProductId = "p1",
                BidAmount = Decimal.Parse("1456.23"),
                BuyerEmail = "rabi@gmail.com"
            };


            buyerDetails = new UserDetail
            {
                Email = "rabi@gmail.com",
                FirstName = "Rabi",
                LastName = "Mandal",
                Address = "32M Jnb",
                City = "Kolkata",
                Phone = "9657123412",
                Pin = "700039",
                State = "WB",
                UserType = "Seller"

            };

            sellerDetails = new UserDetail
            {
                Email = "raja@gmail.com",
                FirstName = "Rabi",
                LastName = "Mandal",
                Address = "32M Jnb",
                City = "Kolkata",
                Phone = "9657123412",
                Pin = "700039",
                State = "WB",
                UserType = "Seller"

            };

            productEntity = new Product
            {
                ProductId = "p1",
                ProductName = "Mobile",
                ShortDescription = "nice mobile",
                DetailDescription = "very nice mobile",
                Category="Ornament",
                BidEndDate = Convert.ToDateTime("12/11/2023"),
                StartingPrice = Decimal.Parse("1234.23"),
                UserDetail = sellerDetails
            };
            productBid = new ProductBid
            {
                BidAmount = Decimal.Parse("1500"),
                Product = productEntity,
                UserDetail = buyerDetails
            };

            IReadOnlyList<ProductBid> productBidsList = new List<ProductBid>{
                productBid
            };

            _mediator.Setup(x => x.Send(It.IsAny<BuyerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(buyerDetails);
            _mediator.Setup(x => x.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(productEntity);
            _mediator.Setup(x => x.Send(It.IsAny<ProductBidSearchByProductIdSellerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(productBid);
            _mediator.Setup(x => x.Send(It.IsAny<ProductBidSearchByProductIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(productBidsList);
            _mediator.Setup(x => x.Send(It.IsAny<AddProductBidCommand>(), It.IsAny<CancellationToken>())).Verifiable();
            _mediator.Setup(x => x.Send(It.IsAny<UpdateProductBidAmountCommand>(), It.IsAny<CancellationToken>())).Verifiable();
        }

        [Fact]
        public async void WhenAddProductBidInvoke_ThenReturnCreated()
        {
            _mediator.Setup(x => x.Send(It.IsAny<ProductBidSearchByProductIdSellerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((ProductBid)null);
            var result = bidsController.Post(request);
            var createdResult = await result as CreatedAtActionResult;
            Assert.NotNull(createdResult);
            Assert.Equal(System.Net.HttpStatusCode.Created, (System.Net.HttpStatusCode)createdResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<BuyerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductBidSearchByProductIdSellerQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<AddProductBidCommand>(), It.IsAny<CancellationToken>()), Times.Once());

        }

        [Fact]
        public async void WhenProductBidAllaredyExistForBuyer_ThenReturnBadRequest()
        {

            var result = bidsController.Post(request);
            var badRequestResult = await result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, (System.Net.HttpStatusCode)badRequestResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<BuyerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductBidSearchByProductIdSellerQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<AddProductBidCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async void WhenInvalidProductIdHasMentinedInProducBidRequest_ThenReturnBadRequest()
        {
            _mediator.Setup(x => x.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product)null);
            var result = bidsController.Post(request);
            var badRequestResult = await result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, (System.Net.HttpStatusCode)badRequestResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<BuyerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(s => s.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductBidSearchByProductIdSellerQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(s => s.Send(It.IsAny<AddProductBidCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async void WhenInvalidBuyerEmailHasMentinedInProducBidRequest_ThenReturnBadRequest()
        {
            _mediator.Setup(x => x.Send(It.IsAny<BuyerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((UserDetail)null);
            var result = bidsController.Post(request);
            var badRequestResult = await result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, (System.Net.HttpStatusCode)badRequestResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<BuyerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductBidSearchByProductIdSellerQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(s => s.Send(It.IsAny<AddProductBidCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async void WhenUdateBidAmount_ThenReturnOk()
        {
            string buyerEmail = "rabi@gmail.com";
            string productId = "p1";
            decimal newBidAmount = decimal.Parse("1560.98");
            var result = bidsController.Put(productId, buyerEmail, newBidAmount);
            var okResult = await result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(System.Net.HttpStatusCode.OK, (System.Net.HttpStatusCode)okResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductBidSearchByProductIdSellerQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<UpdateProductBidAmountCommand>(), It.IsAny<CancellationToken>()), Times.Once());

        }

        [Fact]
        public async void WhenInvalidProductIdAddInUdateBidAmount_ThenReturnBadRequest()
        {
            _mediator.Setup(x => x.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product)null);
            string buyerEmail = "rabi@gmail.com";
            string productId = "p2";
            decimal newBidAmount = decimal.Parse("1560.98");
            var result = bidsController.Put(productId, buyerEmail, newBidAmount);
            var badRequestResult = await result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, (System.Net.HttpStatusCode)badRequestResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductBidSearchByProductIdSellerQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(s => s.Send(It.IsAny<UpdateProductBidAmountCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async void WhenUdateBidAmountOnInvalidProductBid_ThenReturnBadRequest()
        {
            _mediator.Setup(x => x.Send(It.IsAny<ProductBidSearchByProductIdSellerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((ProductBid)null);
            string buyerEmail = "invalid@gmail.com";
            string productId = "p1";
            decimal newBidAmount = decimal.Parse("1560.98");
            var result = bidsController.Put(productId, buyerEmail, newBidAmount);
            var notFoundResult = await result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, (System.Net.HttpStatusCode)notFoundResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductBidSearchByProductIdSellerQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<UpdateProductBidAmountCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async void WhenRetrieveProductBid_ThenReturnOk()
        {
            string productId = "p1";
            var result = bidsController.Get(productId);
            var okResult = await result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(System.Net.HttpStatusCode.OK, (System.Net.HttpStatusCode)okResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductBidSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async void WhenRetrieveProductBidWithInvalidProductId_ThenReturnNotFound()
        {
            _mediator.Setup(x => x.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product)null);
            string productId = "p4";
            var result = bidsController.Get(productId);
            var notFoundResult = await result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, (System.Net.HttpStatusCode)notFoundResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductBidSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}