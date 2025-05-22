using Microsoft.AspNetCore.Identity;
using Presentation.Models;

namespace Presentation.Interfaces;

public interface IAccountService
{
    Task<IdentityUser?> GetUserByEmailAsync(string email);
    Task<AccountServiceResult> SignInAsync(SignInModel form);
    Task<AccountServiceResult> SignUpAsync(SignUpModel form);
    Task<CreateAccountServiceProfileRequest?> ProfilDetailsAsync(string userId);
}