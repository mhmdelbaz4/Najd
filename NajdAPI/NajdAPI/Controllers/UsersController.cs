using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NajdAPI.DTOs.Users;
using NajdAPI.Enums;
using NajdAPI.Models;
using NajdAPI.Repos;

namespace NajdAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IMapper _mapper,UsersRepo _usersRepo): ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser(UserRegisterDTO userDTO)
    {     
        if (await _usersRepo.IsMobileNumberExistAsync(userDTO.MobileNumber))
            return BadRequest("mobile number is already exist!");

        User user = _mapper.Map<User>(userDTO);
        user.PasswordHash = _usersRepo.GenerateHashPassword(userDTO.Password);
        user.VerificationCode = _usersRepo.GenerateVerificationCode();
        user.IsVerified = false;
        _usersRepo.Add(user);

        if (!await _usersRepo.SaveChangesAsync())
            return StatusCode(StatusCodes.Status500InternalServerError, "Something wrong in the server!");

        string token = _usersRepo.GenerateJWTToken(user, Policy.CustomPolicy);
        var responseBody = new { token = token, user = _mapper.Map<UserProfileDTO>(user), verificationCode = user.VerificationCode };

        return Created("/api/users/profile", responseBody);
    }

    [Authorize(policy:"CustomPolicy")]
    [HttpPost("verify-mobile")]
    public async Task<ActionResult> VerifyMobile(VerifyMobileDTO dto)
    {
        User? user =await _usersRepo.GetUserByMobileNumberAsync(dto.MobileNumber);
        if (user == null)
            return BadRequest("Mobile Number isn't registered!");

        bool isEqual = (user.VerificationCode == dto.VerificationCode);
        if (!isEqual)
            return BadRequest("Invalid Verification!");

        user.IsVerified = true;
        user.VerificationCode = string.Empty;
        _usersRepo.Update(user);

        if(!await _usersRepo.SaveChangesAsync())
            return StatusCode(StatusCodes.Status500InternalServerError, "Something wrong in the server!");

        string token = _usersRepo.GenerateJWTToken(user, Policy.GeneralPolicy);
        return Ok(new {token = token});
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> Login(UserAuthDTO dto)
    {
        User? user =await _usersRepo.GetUserByMobileNumberAsync(dto.MobileNumber);
        if(user == null)
            return BadRequest("Mobile number or password isn't correct!");
        
        string token;
        if (! user.IsVerified)
        {
            token = _usersRepo.GenerateJWTToken(user, Policy.CustomPolicy);
            return StatusCode(StatusCodes.Status202Accepted, new { token = token });
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return BadRequest("Mobile number or password is incorrect!");
   

        token = _usersRepo.GenerateJWTToken(user, Policy.GeneralPolicy);
        var responseBody = new { token = token, user = _mapper.Map<UserProfileDTO>(user)};
        return Ok(responseBody);  
    }

    [HttpPost("delete-account")]
    public async Task<ActionResult> DeleteAccount(UserAuthDTO dto)
    {
        User? user = await _usersRepo.GetUserByMobileNumberAsync(dto.MobileNumber);
        if (user == null)
            return BadRequest("Mobile number or password is incorrect!");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return BadRequest("Mobile number or password is incorrect!");

        _usersRepo.Delete(user);
        if (!await _usersRepo.SaveChangesAsync())
            return StatusCode(StatusCodes.Status500InternalServerError, "Something wrong in the server!");
        return Ok();
    }

    [HttpPatch("reset-password")]
    public async Task<ActionResult> ResetPassword(ResetPasswordDTO dto)
    {
        if (dto.Password == dto.NewPassword)
            return BadRequest("new password is equal to old password");
        User? user = await _usersRepo.GetUserByMobileNumberAsync(dto.MobileNumber);
        if (user == null)
            return BadRequest("Mobile number or password is incorrect!");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return BadRequest("Mobile Number or password is incorrect!");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        _usersRepo.Update(user);

        if (!await _usersRepo.SaveChangesAsync())
            return StatusCode(StatusCodes.Status500InternalServerError, "Something wrong in the server!");

        return Ok();
    }

    [HttpPost("forget-password")]
    public async Task<ActionResult> ForgetPassword(ForgetPasswordGetDTO dto)
    {
        User? user =await _usersRepo.GetUserByMobileNumberAsync(dto.MobileNumber);
        if (user == null)
            return BadRequest("Mobile Number is incorrect!");

        if (!user.IsVerified)
        {
            return StatusCode(StatusCodes.Status202Accepted);
        }
        string code = _usersRepo.SendForegetPasswordVerificationCode(user);
        if (!await _usersRepo.SaveChangesAsync())
            return StatusCode(StatusCodes.Status500InternalServerError, "Something wrong in the server!");
        
        return Ok(new { code = code});
    }
    [HttpPatch("forget-password")]
    public async Task<ActionResult> ForgetPassword(ForgetPasswordDTO dto)
    {
        User? user =await _usersRepo.GetUserByMobileNumberAsync(dto.MobileNumber);
        if (user == null)
            return BadRequest("Mobile Number is incorrect!");

        string token = string.Empty;
        if (!user.IsVerified)
        {
            return StatusCode(StatusCodes.Status202Accepted, new { token = token });
        }

        if (user.ForgetPasswordVerificationCode != dto.VerificationCode)
            return BadRequest("Incorrect Verification Code!");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.ForgetPasswordVerificationCode = string.Empty;

        _usersRepo.Update(user);
        if (!await _usersRepo.SaveChangesAsync())
            return StatusCode(StatusCodes.Status500InternalServerError, "Something wrong in the server!");

        token = _usersRepo.GenerateJWTToken(user, Policy.GeneralPolicy);

        return Ok(new { user = _mapper.Map<UserProfileDTO>(user), token = token });
    }

}
