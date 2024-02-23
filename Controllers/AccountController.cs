using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using RahulAI.Model;
using RahulAI.Factory;
using RahulAI.Common;
using System.Text;

namespace RahulAI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AccountController(IUserAuthenticationFactory _userAuthenticationFactory) : Controller
    {
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                    return BadRequest("Password and confirm-password does not match");
                // Check if the email is unique (you might want to add more validation)
                if (_userAuthenticationFactory.UserExist(model.TenantId, model.UserName, model.Email) == false)
                {
                    // Hash the password and save the user to the database
                    var (passwordHash, salt) = HashPassword(model.Password);
                    User user = new User
                    {
                        Id = new Guid(),
                        UserName = model.UserName,
                        EmailId = model.Email,
                        PasswordHash = passwordHash,
                        Saltkey = salt,
                        TenantId = model.TenantId,
                        Name = model.Name
                    };
                    _userAuthenticationFactory.CreateUser(user);
                    return Ok(new { user.Id });
                }
                else
                {
                    return BadRequest("Email or userName is already taken.");
                }
            }

            return BadRequest("Invalid model data.");
        }

        [HttpPost("login")]
        public IActionResult login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userAuthenticationFactory.GetUser(null, model.UserName);
                if (user != null && ValidatePassword(model.Password, user.PasswordHash, user.Saltkey))
                {
                    // Password is valid, perform login logic (you might want to use a cookie or JWT)
                    Claim[] claims = new Claim[]
                    {
                        new ("name", user.Name??string.Empty),
                        new ("userName", user.UserName),
                        new ("userId", user.Id.ToString()),
                        new ("email", user.EmailId ?? string.Empty),
                        new ("tenantId", user?.TenantId.ToString()??string.Empty)
                    };
                    var token = GenerateJSONWebToken(claims, AppSetting.TokenExpirationtime);
                    var key = generateSecretKey();
                    Claim[] newClaims = claims.Append(new Claim("secretKey", key)).ToArray();
                    var reftoken = GenerateJSONWebToken(newClaims, AppSetting.TokenExpirationtime  * 2);
                    _userAuthenticationFactory.CreateRefreshTokenByUser(user.TenantId, user.Id, key);
                    return Ok(new{
                        token,
                        refreshtoken = reftoken,
                        tokenType = "Bearer"
                    });
                }

                return Unauthorized("Invalid login attempt.");
            }

            return BadRequest("Invalid model.");
        }

        [HttpPost("refresh")]
        public IActionResult refresh([FromBody] Token refreshtoken)
        {
            IActionResult response = Unauthorized();
            var handler = new JwtSecurityTokenHandler();
            var usertoken = handler.ReadToken(refreshtoken.RefreshToken) as JwtSecurityToken;
            var userid = usertoken.Claims.First(claim => claim.Type == "userId").Value;
            var tenantid = usertoken.Claims.First(claim => claim.Type == "tenantId").Value;
            var tokenstring = usertoken.Claims.First(claim => claim.Type == "secretKey").Value;
            var tokenId = ValidateRefreshToken(usertoken, tokenstring, Guid.Parse(tenantid), Guid.Parse(userid));
            if (tokenId != Guid.Empty)
            {
                var user = _userAuthenticationFactory.GetUser(Guid.Parse(userid), string.Empty);
                Claim[] claims = new Claim[]
                {
                    new ("name", user.Name??string.Empty),
                    new ("userName", user.UserName),
                    new ("userId", user.Id.ToString()),
                    new ("email", user.EmailId ?? string.Empty),
                    new ("tenantId", user?.TenantId.ToString()??string.Empty)
                };
                var token = GenerateJSONWebToken(claims, AppSetting.TokenExpirationtime);
                var key = generateSecretKey();
                Claim[] newClaims = claims.Append(new Claim("secretKey", key)).ToArray();
                var reftoken = GenerateJSONWebToken(newClaims, AppSetting.TokenExpirationtime  * 2);
                _userAuthenticationFactory.UpdateRefreshTokenByUser(user.TenantId, tokenId, user.Id, key);
                if (claims != null)
                {
                    return Ok(new{
                        token,
                        refreshtoken = reftoken,
                        tokenType = "Bearer"
                    });
                }
            }

            return response;
        }

        private Guid ValidateRefreshToken(JwtSecurityToken token, string tokenstring, Guid tenantid, Guid userid)
        {
            var userTokenId = _userAuthenticationFactory.RefreshTokenExist(tenantid, tokenstring, userid);
            if (userTokenId == null || userTokenId == Guid.Empty)
            {
                throw new SecurityTokenException("Invalid token!");
            }

            var expDate = token.ValidTo;
            if (expDate < DateTime.UtcNow.AddMinutes(1))
            {
                throw new SecurityTokenException("Invalid token!");
            }

            return (Guid)userTokenId;
        }

        static (string, string) HashPassword(string password)
        {
            var saltBytes = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[saltBytes.Length + passwordBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, saltedPassword, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, saltBytes.Length, passwordBytes.Length);
            var hashedBytes = SHA256.HashData(saltedPassword);
            return (Convert.ToBase64String(hashedBytes), Convert.ToBase64String(saltBytes));
        }

        static bool ValidatePassword(string password, string storedHash, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[saltBytes.Length + passwordBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, saltedPassword, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, saltBytes.Length, passwordBytes.Length);
            byte[] hashedBytes = SHA256.HashData(saltedPassword);
            string computedHash = Convert.ToBase64String(hashedBytes);
            return storedHash == computedHash;
        }

        private readonly static string jwtKey = !string.IsNullOrEmpty(AppSetting.JwtKey) ? AppSetting.JwtKey : string.Empty;

        protected static string GenerateJSONWebToken(Claim[] claims, int expiryTime)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(expiryTime);
            var token = new JwtSecurityToken(AppSetting.JwtIssuer, AppSetting.JwtIssuer, claims,  expires: expirationTime.ToUniversalTime(),  signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string generateSecretKey()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}