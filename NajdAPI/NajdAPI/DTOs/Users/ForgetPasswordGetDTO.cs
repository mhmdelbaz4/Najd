using System.ComponentModel.DataAnnotations;

namespace NajdAPI.DTOs.Users
{
    public class ForgetPasswordGetDTO
    {
        [Required]
        [StringLength(15)]
        [RegularExpression(@"^\+9665[0-9]{8}$", ErrorMessage = "Incorrect Mobile Number!")]
        public string MobileNumber { get; set; } = string.Empty;
    }
}
