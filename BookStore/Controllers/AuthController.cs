using BookStore.Data;
using BookStore.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BookStoreDbContext _bookStoreDbContext;

        public AuthController(BookStoreDbContext context)
        {
            _bookStoreDbContext = context;
        }

        [HttpPost("Signup")]
        public async Task<ActionResult<User>> Signup([FromBody] User user)
        {
            if (user == null || user.Email == null || user.Password == null)
            {
                return BadRequest("Invalid client request");
            }
            Guid guid = Guid.NewGuid();
            User userNew = new User();
            userNew.Id = guid;
            userNew.Email = user.Email;
            userNew.Password = user.Password;
            userNew.Role = user.Role;
            userNew.Name = user.Name;
            _bookStoreDbContext.Users.Add(userNew);
            await _bookStoreDbContext.SaveChangesAsync();
            return Ok(userNew);
        }

        [HttpPost("Login")]
        public ActionResult<AuthenticatedUserResponse> Login([FromBody] LoginModel loginModel)
        {
            if (loginModel == null || loginModel.Email == null || loginModel.Password == null)
            {
                return BadRequest("Invalid client request");
            }
            User user = _bookStoreDbContext.Users.Where(user => user.Email == loginModel.Email && user.Password == loginModel.Password).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("User was not found");
            }
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@mk34superSecretKey@mk34"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: "https://localhost:7292",
                audience: "https://localhost:7292",
                claims: new List<Claim>(),
                expires: DateTime.UtcNow.AddDays(2),
                signingCredentials: signinCredentials
            );
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return Ok(new AuthenticatedUserResponse { Token = tokenStr , User = user });
        }
    }
}
