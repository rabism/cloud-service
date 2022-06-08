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
    public class ProductControllerTest
    {
        ProductController productController;
        Mock<IMediator> _mediator;
        Mock<ILogger<ProductController>> _logger;
        UserDetail userDetails;
        ProductDetail productDetailRequest;
        Product productEntity;
        IReadOnlyList<ProductBid> bidList;
        public ProductControllerTest()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<ProductController>>();
            setUpMock();
            productController = new ProductController(_logger.Object, _mediator.Object);
        }
        void setUpMock()
        {

            productDetailRequest = new ProductDetail
            {
                ProductId = "p1",
                ProductName = "Mobile",
                ShortDescription = "nice mobile",
                DetailDescription="very nice mobile",
                Category = "Ornament",
                BidEndDate = Convert.ToDateTime("12/11/2023"),
                StartingPrice = Decimal.Parse("1234.23"),
                SellerEmail = "rabi@gmail.com",
            };

            userDetails = new UserDetail
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

            productEntity = new Product
            {
                ProductId = "p1",
                ProductName = "Mobile",
                ShortDescription = "nice mobile",
                DetailDescription="very nice mobile",
                Category="Ornament",
                BidEndDate = Convert.ToDateTime("12/11/2023"),
                StartingPrice = Decimal.Parse("1234.23"),
                UserDetail = userDetails
            };

            bidList = new List<ProductBid>(){
            new ProductBid{
                BidAmount=Decimal.Parse("1500"),
                Product=productEntity,
                UserDetail=userDetails
            }

            };

            _mediator.Setup(x => x.Send(It.IsAny<SellerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(userDetails);
            _mediator.Setup(x => x.Send(It.IsAny<AddProductCommand>(), It.IsAny<CancellationToken>())).Verifiable();
            _mediator.Setup(x => x.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(productEntity);
            _mediator.Setup(x => x.Send(It.IsAny<ProductBidSearchByProductIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(bidList);
            _mediator.Setup(x => x.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>())).Verifiable();
        }
        [Fact]
        public async void WhenAddProductInvoke_ThenReturnCreated()
        {
            var result = productController.Post(productDetailRequest);
            var createdResult = await result as CreatedAtActionResult;
            Assert.NotNull(createdResult);
            Assert.Equal(System.Net.HttpStatusCode.Created, (System.Net.HttpStatusCode)createdResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<SellerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<AddProductCommand>(), It.IsAny<CancellationToken>()), Times.Once());

        }

        [Fact]
        public async void WhenAddProductInvokeWithInvalidSellerEmail_ThenReturnCreated()
        {
            string invalidSellerEmail = "invalid@gmail.com";
            var sellerDetailSearchByEmailQuery = new SellerDetailSearchByEmailQuery { SellerEmail = invalidSellerEmail };
            _mediator.Setup(x => x.Send(It.IsAny<SellerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((UserDetail)null);
            productDetailRequest.SellerEmail = invalidSellerEmail;
            var result = productController.Post(productDetailRequest);
            var badResult = await result as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, (System.Net.HttpStatusCode)badResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<SellerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<AddProductCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async void WhenAddProductInvokeWithUnhandleException_ThenReturn500()
        {
            _mediator.Setup(x => x.Send(It.IsAny<SellerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NullReferenceException("object is null"));
            var result = productController.Post(productDetailRequest);
            var internalErrorResult = await result as StatusCodeResult;
            Assert.NotNull(internalErrorResult);
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, (System.Net.HttpStatusCode)internalErrorResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<SellerDetailSearchByEmailQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<AddProductCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async void WhenAddDeleteInvoke_ThenReturnOk()
        {
            _mediator.Setup(x => x.Send(It.IsAny<ProductBidSearchByProductIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((List<ProductBid>)null);
            string productId = "p1";
            var result = productController.Delete(productId);
            var okResult = await result as OkResult;
            Assert.NotNull(okResult);
            Assert.Equal(System.Net.HttpStatusCode.OK, (System.Net.HttpStatusCode)okResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductBidSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()), Times.Once());

        }


        [Fact]
        public async void WhenAddDeleteInvokeWithProductHaveBid_ThenReturnBadRequest()
        {
            string productId = "p1";
            var result = productController.Delete(productId);
            var badResult = await result as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, (System.Net.HttpStatusCode)badResult.StatusCode);
            _mediator.Verify(s => s.Send(It.IsAny<ProductSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<ProductBidSearchByProductIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
            _mediator.Verify(s => s.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        }

    }
}
