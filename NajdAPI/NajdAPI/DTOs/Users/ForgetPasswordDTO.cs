using System.ComponentModel.DataAnnotations;

namespace NajdAPI.DTOs.Users
{
    public class ForgetPasswordDTO
    {
        [Required]
        [StringLength(15)]
        [RegularExpression(@"^\+9665[0-9]{8}$", ErrorMessage = "Incorrect Mobile Number!")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; } = string.Empty;

        [Required]
        [StringLength (6,MinimumLength =6)]
        public string VerificationCode { get; set; } = string.Empty;


    }
}
