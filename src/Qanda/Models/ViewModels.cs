using System;
using System.ComponentModel.DataAnnotations;

namespace Qanda.ViewModels;

public class SignInViewModel
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public bool RememberMe { get; set; }
}

public class SignUpViewModel
{
    [Required]
    [MinLength(5, ErrorMessage = "Username is invalid! It must be at least 5 characters long")]
    [MaxLength(16, ErrorMessage = "Password is invalid! It must be at most 16 characters long")]
    public string? Username { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password is invalid! It must be at least 8 characters long")]
    public string? Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password is invalid! It must be at least 8 characters long")]
    public string? ConfirmPassword { get; set; }
}

public class AskQuestionViewModel
{
    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Body { get; set; }
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}