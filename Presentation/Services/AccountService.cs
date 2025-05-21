using Microsoft.AspNetCore.Identity;
using Presentation.Interfaces;
using Presentation.Models;

namespace Presentation.Services;

public class AccountService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : IAccountService
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public async Task<AccountServiceResult> SignUpAsync(SignUpModel form)
    {
        try
        {
            var existingAccount = await _userManager.FindByEmailAsync(form.Email);
            if (existingAccount != null)
                return new AccountServiceResult { Succeeded = false, StatusCode = 404, Message = "Email allready in use" };

            var user = new IdentityUser { UserName = form.Email, Email = form.Email, EmailConfirmed = form.EmailConfirmed };
            var result = await _userManager.CreateAsync(user, form.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }
                var addRoleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!addRoleResult.Succeeded)
                {
                    return new AccountServiceResult { Succeeded = false, Message = "Error adding role to user" };
                }
            }
            return new AccountServiceResult { Succeeded = true, StatusCode = 200, Message = "User and role added in database!" };
        }
        catch (Exception ex)
        {
            return new AccountServiceResult { Succeeded = false, StatusCode = 500, Message = $"There is a problem. The person who wrote this code does not know what the problem is:) {ex.Message}" };

        }
    }

    public async Task<AccountServiceResult> SignInAsync(SignInModel form)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(form.Email);
            if (user == null)
                return new AccountServiceResult { Succeeded = false, StatusCode = 404, Message = "Email not found" };
            var result = await _userManager.CheckPasswordAsync(user, form.Password);
            if (!result)
                return new AccountServiceResult { Succeeded = false, StatusCode = 401, Message = "Invalid password." };
            return new AccountServiceResult { Succeeded = true, StatusCode = 200, Message = "User signed in successfully." };
        }
        catch (Exception ex)
        {
            return new AccountServiceResult { Succeeded = false, StatusCode = 500, Message = $"There is a problem. The person who wrote this code does not know what the problem is:) {ex.Message}" };
        }
    }

    public async Task<IdentityUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
}