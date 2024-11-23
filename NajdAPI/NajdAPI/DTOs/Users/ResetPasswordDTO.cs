using System.ComponentModel.DataAnnotations;

namespace NajdAPI.DTOs.Users
{
    public class ResetPasswordDTO
    {
        [Required]
        [StringLength(15)]
        [RegularExpression(@"^\+9665[0-9]{8}$", ErrorMessage = "Incorrect Mobile Number!")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
