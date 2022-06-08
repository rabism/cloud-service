using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using user.Models;
using user.Services;

namespace user.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        readonly IUserService service;
        readonly IConfiguration _configuration;
        readonly ILogger<AuthenticationController> _logger;
        readonly IMessageProducerService _messageService;
        public AuthenticationController(IUserService userService,
        IConfiguration configuration, ILogger<AuthenticationController> logger,
        IMessageProducerService messageService
        )
        {
            service = userService;
            _configuration = configuration;
            _logger = logger;
            _messageService = messageService;
        }

        [HttpPost("v1/users/authenticate")]
        public IActionResult CheckValidUser([FromBody] UserAuthDetail userAuthDetail)
        {
            try
            {
                _logger.LogInformation($"Verifying user details and generate token");
                var _user = service.Login(userAuthDetail.Email, userAuthDetail.Password);
                return Ok(GetJWTToken(_user.Email,_user.UserType));
            }
            catch (UserNotFoundException unf)
            {
                _logger.LogInformation($"Provided credentials are not correct");
                return NotFound(unf.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in verifying the user");
                _logger.LogError(e.StackTrace);
                return StatusCode(500);
            }
        }

        [HttpPost("v1/users")]
        [ActionName("Post")]
        public async Task<IActionResult> Post([FromBody] UserDetails user)
        {
            try
            {
                _logger.LogInformation($"Start controller {JsonConvert.SerializeObject(user)}");
                _logger.LogInformation($"Adding a new user {user}");
                service.AddUser(user);
                await Task.Run(() => ProduceMessage(user));
                _logger.LogInformation("save successfully");
                return Created("", "User is registered successfully");
            }
            catch (UserAlreadyExistsException usr)
            {
                _logger.LogInformation($"This user already exists {user.Email}");
                return Conflict(usr.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Adding the user: {ex.Message}");
                return StatusCode(500,ex.StackTrace);
            }
        }

        private void ProduceMessage(UserDetails user)
        {
            UserBasicDetails userBasicDetails = new UserBasicDetails
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                City = user.City,
                State = user.State,
                Pin = user.Pin,
                Phone = user.Phone,
                UserType=user.UserType
            };
            string userTopic = _configuration.GetSection("Producer:UserTopic").Value;
            if (string.IsNullOrEmpty(userTopic))
            {
                string errorMessage = "Topic can't be null or empty..";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }
            _messageService.WriteMessage(userTopic, userBasicDetails);

        }
        private string GetJWTToken(string userId, string userType)
        {
            //setting the claims for the user credential name
            var claims = new[]
           {
                new Claim("userId", userId),
            };

            //Defining security key and encoding the claim 

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            //defining the JWT token essential information and setting its expiration time
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );
            //defing the response of the token 
            var response = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                userName = userId,
                userType=userType
            };
            //convert into the json by serialing the response object
            return JsonConvert.SerializeObject(response);
        }
    }
}
