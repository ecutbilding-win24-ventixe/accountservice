using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class SignUpModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = null!;

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;

    public bool EmailConfirmed { get; set; } = false;
}
