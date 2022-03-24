using System.ComponentModel.DataAnnotations;

namespace Qanda.ViewModels;
public class EmailSettingsViewModel
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? NewEmail { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password is invalid! It must be at least 8 characters long")]
    public string? Password { get; set; }
}