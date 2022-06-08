using System;
using Xunit;
using user.Controllers;
using Moq;
using user.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using user.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit.Sdk;
using System.Text.Json;
using System.Threading.Tasks;

namespace user.test
{
    public class AuthenticationControllerTest
    {
        AuthenticationController authenticationController;
        Mock<IUserService> userService;
        Mock<IConfiguration> configuration;
        Mock<ILogger<AuthenticationController>> logger;
        Mock<IMessageProducerService> messageService;
        UserDetails userDetails;
        public AuthenticationControllerTest()
        {

            userService = new Mock<IUserService>();
            configuration = new Mock<IConfiguration>();
            logger = new Mock<ILogger<AuthenticationController>>();
            messageService = new Mock<IMessageProducerService>();
            setUpMock();
            authenticationController = new AuthenticationController(userService.Object, configuration.Object, logger.Object, messageService.Object);
        }

        void setUpMock()
        {
            var topicSectionMock = new Mock<IConfigurationSection>();
            topicSectionMock
               .Setup(x => x.Value)
               .Returns("User");
            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock
               .Setup(x => x.Value)
               .Returns("secret_auth_jwt_to_secure_microservice");
            configuration
               .Setup(x => x.GetSection(It.IsAny<string>()))
               .Returns((string key) =>
                   {
                       Console.WriteLine(key);
                       if (key == "JWT:Secret")
                       {
                           return jwtSectionMock.Object;
                       }
                       else if (key == "Producer:UserTopic")
                       {
                           return topicSectionMock.Object;
                       }
                       return null;
                   });
            // .Returns(configurationSectionMock.Object);
            //x=>x=="JWT:Secret"?configurationSectionMock.Object:topicSectionMock.Object
            userDetails = new UserDetails
            {
                Email = "rabi@gmail.com",
                FirstName = "Rabi",
                LastName = "Mandal",
                Address = "32M Jnb",
                City = "Kolkata",
                Phone = "9657123412",
                Pin = "700039",
                State = "WB",
                UserType = "Seller",
                Password = "password"
            };
            userService.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).Returns(userDetails);
            userService.Setup(x => x.AddUser(It.IsAny<UserDetails>())).Verifiable();
            messageService.Setup(x => x.WriteMessage(It.IsAny<string>(), It.IsAny<UserBasicDetails>())).Verifiable();
        }

        [Fact]
        public void WhenCheckValidUserInvoke_ThenReturnOk()
        {
            var request = new UserAuthDetail { Email = "rabi@gmail.com", Password = "password" };
            var result = authenticationController.CheckValidUser(request);
            var okResult = result as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal(200, okResult.StatusCode);
            userService.Verify(s => s.Login(request.Email, request.Password), Times.Once());
        }

        [Fact]
        public void WhenCheckValidUserInvokeWithInvalidCredential_ThenReturnNotFound()
        {
            userService.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).Throws(new UserNotFoundException("Invalid User"));
            var request = new UserAuthDetail { Email = "rabii@gmail.com", Password = "password" };
            var result = authenticationController.CheckValidUser(request);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            userService.Verify(s => s.Login(request.Email, request.Password), Times.Once());
        }

        [Fact]
        public void WhenCheckValidUserInvokeWithUnhandleException_ThenReturn500Code()
        {
            userService.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).Throws(new NullReferenceException("object is null"));
            var request = new UserAuthDetail { Email = "rabii@gmail.com", Password = "password" };
            var result = authenticationController.CheckValidUser(request);
            var internalErrorResult = result as StatusCodeResult;
            Assert.NotNull(result);
            Assert.Equal(500, internalErrorResult.StatusCode);
            userService.Verify(s => s.Login(request.Email, request.Password), Times.Once());
        }

        [Fact]
        public async void WhenAddNewUserInvoke_ThenReturnCreated()
        {
            var request = userDetails;
            var result = authenticationController.Post(request);
            var createdResult = await result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.IsType<CreatedResult>(createdResult);
            Assert.Equal(System.Net.HttpStatusCode.Created, (System.Net.HttpStatusCode)createdResult.StatusCode);
            userService.Verify(s => s.AddUser(request), Times.Once());
            messageService.Verify(s => s.WriteMessage(It.IsAny<string>(), It.IsAny<UserBasicDetails>()), Times.Once());
        }

        [Fact]
        public async void WhenAddNewUserInvokeWithSameUserInformation_ThenReturnConflict()
        {
            var request = userDetails;
            userService.Setup(x => x.AddUser(It.IsAny<UserDetails>())).Throws(new UserAlreadyExistsException("User already exist"));
            var result = authenticationController.Post(request);
            var conflictResult = await result as ConflictObjectResult;
            Assert.NotNull(conflictResult);
            Assert.IsType<ConflictObjectResult>(conflictResult);
            Assert.Equal(System.Net.HttpStatusCode.Conflict, (System.Net.HttpStatusCode)conflictResult.StatusCode);
            userService.Verify(s => s.AddUser(request), Times.Once());
            messageService.Verify(s => s.WriteMessage(It.IsAny<string>(), It.IsAny<UserBasicDetails>()), Times.Never);
        }

         [Fact]
        public async void WhenAddNewUserInvokeWithUnhandaleException_ThenReturn500()
        {
            var request = userDetails;
            userService.Setup(x => x.AddUser(It.IsAny<UserDetails>())).Throws(new NullReferenceException("object is null"));
            var result = authenticationController.Post(request);
            var errorResult = await result as ObjectResult;
            Assert.NotNull(errorResult);
            Assert.IsType<ObjectResult>(errorResult);
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, (System.Net.HttpStatusCode)errorResult.StatusCode);
            userService.Verify(s => s.AddUser(request), Times.Once());
            messageService.Verify(s => s.WriteMessage(It.IsAny<string>(), It.IsAny<UserBasicDetails>()), Times.Never);
        }
    }
}
