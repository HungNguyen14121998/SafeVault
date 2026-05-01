using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SafeVaultApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;

        // old way
        private readonly UserRepository _repo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _logger = logger;

            string connStr = config.GetConnectionString("DefaultConnection");
            _repo = new UserRepository(connStr);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // old way
            // bool success = _repo.AuthenticateUser(username, password);

            // new way
            var user = await _userManager.FindByNameAsync(loginDto.username);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.password))
            {
                _logger.LogInformation("User {Username} logged in successfully at {Time}.", loginDto.username, DateTime.UtcNow);

                var token = GenerateJwtToken(user.UserName);
                return Ok(new { token });
            }

            _logger.LogWarning("Failed login attempt for user {Username} at {Time}.", loginDto.username, DateTime.UtcNow);
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto regiserDto)
        {
            // old way
            // bool success = _repo.RegisterUser(username, email, password);

            if (!await _roleManager.RoleExistsAsync(regiserDto.role))
            {
                await _roleManager.CreateAsync(new IdentityRole(regiserDto.role));
                _logger.LogInformation("Role {Role} created at {Time}.", regiserDto.role, DateTime.UtcNow);
            }

            var user = new IdentityUser
            {
                UserName = regiserDto.username,
                Email = regiserDto.email
            };
            var result = await _userManager.CreateAsync(user, regiserDto.password);

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to register user {Username} at {Time}. Errors: {Errors}",
                    regiserDto.username, DateTime.UtcNow, result.Errors);
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, regiserDto.role);
            _logger.LogInformation("User {Username} registered with role {Role} at {Time}.",
                regiserDto.username, regiserDto.role, DateTime.UtcNow);

            return Ok($"User {regiserDto.username} registered with role {regiserDto.role}");
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
