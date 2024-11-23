using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.IdentityModel.Tokens;
using NajdAPI.Data;
using NajdAPI.DTOs.Users;
using NajdAPI.Enums;
using NajdAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NajdAPI.Repos
{
    public class UsersRepo(NajdDBContext _context, IConfiguration _config) : GenericRepo<User>(_context)
    {
        public string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000,999999).ToString();
        }
        public string GenerateHashPassword(string password)
        {
            string hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashPassword;
        }
        public string GenerateJWTToken(User user, Policy policy)
        {
            string policyKey = (policy == Policy.GeneralPolicy) ? "JWT:SecurityKey-general" : "JWT:SecurityKey-custom";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection(policyKey).Value!));
                    
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>()
            {
                new Claim("Id",user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.MobilePhone, user.MobileNumber)
            };
            int hours = int.Parse(_config.GetSection("JWT:ExpiresDateInHours").Value!);
            var jwt = new JwtSecurityToken(
                    claims: claims,
                    signingCredentials: cred,
                    expires: DateTime.UtcNow.AddHours(hours));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        public async Task<bool> IsMobileNumberExistAsync(string mobileNo)
        {
            bool isExist =await _context.Users.AnyAsync(user => user.MobileNumber == mobileNo && user.IsDeleted == false);
            return isExist;
        }
        public async Task<User?> GetUserByMobileNumberAsync(string mobileNo)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(user => user.MobileNumber == mobileNo && user.IsDeleted == false);
            return user;
        }

        public string SendForegetPasswordVerificationCode(User user)
        {
            string code = GenerateVerificationCode();
            // send sms message
            user.ForgetPasswordVerificationCode = code;
            Update(user);
            return code;
        }
    
       
    }
}
