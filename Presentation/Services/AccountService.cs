using Microsoft.AspNetCore.Identity;
using Presentation.Interfaces;
using Presentation.Models;

namespace Presentation.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly HttpClient _httpClient;

    public AccountService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, HttpClient httpClient)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://ventixe-accountserviceprofile-exdnhxd3d0hkfcbj.swedencentral-01.azurewebsites.net");
    }

    public async Task<AccountServiceResult> SignUpAsync(SignUpModel form)
    {
        try
        {
            var existingAccount = await _userManager.FindByEmailAsync(form.Email);
            if (existingAccount != null)
                return new AccountServiceResult { Succeeded = false, StatusCode = 404, Message = "Email allready in use" };

            var user = new IdentityUser { UserName = form.Email, Email = form.Email, EmailConfirmed = form.EmailConfirmed };
            var result = await _userManager.CreateAsync(user, form.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AccountServiceResult { Succeeded = false, StatusCode = 400, Message = errors };
            }

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            var addRoleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!addRoleResult.Succeeded)
            {
                return new AccountServiceResult { Succeeded = false, Message = "Error adding role to user" };
            }

            var userProfileRequest = new CreateAccountServiceProfileRequest
            {
                UserId = user.Id,
                FirstName = form.FirstName,
                LastName = form.LastName,
            };

            
            var response = await _httpClient.PostAsJsonAsync("/api/AccountProfileService/create-profile", userProfileRequest);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return new AccountServiceResult { Succeeded = false, StatusCode = (int)response.StatusCode, Message = $"Error creating user profile: {errorMessage}" };
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