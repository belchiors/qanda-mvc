using System.ComponentModel.DataAnnotations;

namespace Qanda.ViewModels;
public class PasswordSettingsViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password is invalid! It must be at least 8 characters long")]
    public string? CurrentPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password is invalid! It must be at least 8 characters long")]
    public string? NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password is invalid! It must be at least 8 characters long")]
    public string? ConfirmPassword { get; set; }
}