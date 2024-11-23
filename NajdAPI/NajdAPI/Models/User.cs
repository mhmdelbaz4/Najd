using System.ComponentModel.DataAnnotations;
namespace NajdAPI.Models;

public class User : BaseEntity
{
    [StringLength(50)]
    public string Email { get; set; } = string.Empty;
    [StringLength(100)]
    public string PasswordHash { get;set; }= string.Empty;
    [StringLength(15)]
    public string MobileNumber { get; set; } = string.Empty;
    [StringLength(6)]
    public string VerificationCode { get; set; } = string.Empty;
    [StringLength(6)]
    public string ForgetPasswordVerificationCode { get;set; } = string.Empty;
    public bool IsVerified { get; set; }
}
