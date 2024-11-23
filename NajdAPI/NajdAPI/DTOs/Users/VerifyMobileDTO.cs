using System.ComponentModel.DataAnnotations;

namespace NajdAPI.DTOs.Users;

public class VerifyMobileDTO
{
    [Required]
    [StringLength(15)]
    [RegularExpression(@"^\+9665[0-9]{8}$", ErrorMessage = "Incorrect Mobile Number!")]
    public string MobileNumber { get; set; } = string.Empty;
    [Required]
    [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Verification Code should be 6 digits!")]
    public string VerificationCode { get; set; } = string.Empty;
}
