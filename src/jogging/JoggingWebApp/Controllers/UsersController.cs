using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using BusinessLogic.Common;
using BusinessLogic.Exceptions;
using BusinessLogic.Managers;
using Domain;
using JoggingWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JoggingWebApp.Controllers
{
    [Authorize]
    [ApiController]
    [HasPermission(Permissions.ManageUsers)]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly AppSettings _appSettings;

        public UsersController(IUserManager userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }
        
        [HttpGet]
        public IActionResult Get(int? skip = null, int take = 10, string filter = null)
        {
            if (skip < 0 || take < 1)
            {
                return BadRequest();
            }

            if (take > 50)
            {
                return BadRequest("Can't have more than 50 records on a page");
            }

            var paging = _userManager.Get(filter, take, skip);
            return Ok(paging.Select(ToModel));
        }
        
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(long id)
        {
            if (id <= 0)
            {
                return BadRequest("asd");
            }

            var user = _userManager.Get(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            
            return Ok(ToModel(user));
        }

        [HttpPost]
        public IActionResult Add([FromBody] UpdateUserModel model)
        {
            ValidateUser(model);
            
            var user = FromModel(model);
            _userManager.Add(user);
            
            return CreatedAtAction(nameof(Get), new { user.Id }, ToModel(user));
        }
        
        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(long id, [FromBody] UpdateUserModel model)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            
            ValidateUser(model);
            
            var user = FromModel(model);
            user.Id = id;
            _userManager.Update(user);
            
            return NoContent();
        }
        
        [HttpPatch]
        [Route("{id}")]
        public IActionResult Patch(long id, [FromBody] UpdateUserModel model)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            
            var user = FromModel(model);
            user.Id = id;
            _userManager.Update(user);
            
            return NoContent();
        }
        
        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            
            _userManager.Delete(id);

            return NoContent();
        }

        [HttpPut]
        [Route("{userId}/Role/{roleId}")]
        public IActionResult UpdateRole(long userId, int roleId)
        {
            if (userId <= 0 || roleId <= 0)
            {
                return BadRequest();
            }

            _userManager.UpdateRole(userId, roleId);

            return NoContent();
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("auth")]
        public IActionResult Auth([FromBody] AuthModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest();
            }
            
            var user = _userManager.FindUser(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var passwordValid = _userManager.ComparePassword(user, model.Password);
            if (!passwordValid)
            {
                return BadRequest("Invalid password");
            }

            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(nameof(Permissions), string.Join(",", user.Role.Permissions.Select(p => p.Permission.Id)))
                }),
                Expires = DateTime.UtcNow.AddDays(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                user.Id,
                user.UserName,
                Token = tokenString,
                Permissions = user.Role.Permissions.Select(p => p.Permission.Id)
            });
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public IActionResult Register(UpdateUserModel model)
        {
            ValidateUser(model);

            var user = FromModel(model);
            _userManager.Add(user);
            
            return CreatedAtAction(nameof(Get), new { user.Id }, ToModel(user));
        }
        
        private void ValidateUser(UpdateUserModel model)
        {
            if (model == null)
            {
                throw new BadRequestException();
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new BadRequestException("Email should not be empty");
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new BadRequestException("Password should not be empty");
            }

            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                throw new BadRequestException("User name should not be empty");
            }
        }

        private GetUserModel ToModel(User user)
        {
            return new GetUserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role.Name
            };
        }
        
        private User FromModel(UpdateUserModel updateUserModel)
        {
            return new User
            {
                UserName = updateUserModel.UserName,
                Email = updateUserModel.Email,
                Password = updateUserModel.Password
            };
        }
    }
}