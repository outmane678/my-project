using System.ComponentModel.DataAnnotations;

namespace user_account_service.DTOs.Requests;

public record SignupRequestDto(
    [Required(ErrorMessage = "username is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "min is 2 characters and max is 50 characters")]
    string Username,

    [Required(ErrorMessage = "password is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "min is 2 characters and max is 50 characters")]
    string Password
);