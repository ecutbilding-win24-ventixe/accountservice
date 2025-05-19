using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Presentation.Services;
using System.Net.WebSockets;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(UserManager<IdentityUser> userManager) : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager = userManager;

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingAccount = await _userManager.FindByEmailAsync(model.Email);
        if (existingAccount != null)
            return BadRequest("Email already in use.");

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new AccountServiceResult { Succeeded = false, StatusCode = 400, Message = errors });
        }

        return Ok(new AccountServiceResult { Succeeded = true, StatusCode = 200, Message = "User created successfully." });
    }

}