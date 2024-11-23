using System.ComponentModel.DataAnnotations;

namespace NajdAPI.DTOs.Users;

public class UserRegisterDTO
{
    [Required]
    [StringLength(50)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    [RegularExpression(@"^\+9665[0-9]{8}$", ErrorMessage = "Incorrect Mobile Number!")]
    public string MobileNumber { get; set; } = string.Empty;
}
