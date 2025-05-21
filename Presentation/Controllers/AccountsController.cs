using Microsoft.AspNetCore.Mvc;
using Presentation.Helper;
using Presentation.Interfaces;
using Presentation.Models;


namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(IAccountService accountService, IConfiguration configuration) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _accountService.SignUpAsync(model);
        if (!result.Succeeded)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.SignInAsync(model);
        if (!result.Succeeded)
            return StatusCode(result.StatusCode, result);

        var user = await _accountService.GetUserByEmailAsync(model.Email);
        var tokenGenerator = new GenerateJWTToken(_configuration);
        var token = tokenGenerator.GenerateToken(new User { Id = user!.Id, Email = user.Email! });

        return Ok(new { Succeeded = true, token, email = user.Email, message = "User signed in succesfully...." });
    }

    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmailExists([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email))
            return BadRequest(new { Exists = false, Message = "Email is required." });

        var user = await _accountService.GetUserByEmailAsync(email.Trim().ToLower());

        if (user != null)
            return Ok(new { Exists = true, Message = "Email already exists." });

        return Ok(new { Exists = false, Message = "Email is available." });
    }

}



//[HttpPost("signup")]
//    public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ModelState);

//        var existingAccount = await _userManager.FindByEmailAsync(model.Email);
//        if (existingAccount != null)
//            return BadRequest("Email already in use.");

//        var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = model.EmailConfirmed };

//        var result = await _userManager.CreateAsync(user, model.Password);

//        if (!result.Succeeded)
//        {
//            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
//            return BadRequest(new AccountServiceResult { Succeeded = false, StatusCode = 400, Message = errors });
//        }

//        return Ok(new AccountServiceResult { Succeeded = true, StatusCode = 200, Message = "User created successfully." });
//    }

//    [HttpPost("signin")]
//    public async Task<IActionResult> SignIn([FromBody] SignInModel model)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ModelState);

//        var user = await _userManager.FindByEmailAsync(model.Email);
//        if (user == null)
//            return NotFound("Email not found");

//        var result = await _userManager.CheckPasswordAsync(user, model.Password);
//        if (!result)
//            return Unauthorized("Invalid password.");

//        var tokenGenerator = new GenerateJWTToken(_configuration);
//        var token = tokenGenerator.GenerateToken(new User { Id = user.Id, Email = user.Email! });

//        return Ok(new { Succeeded = true, token, email = user.Email, message = "User signed in succesfully...." });
//    }

//    [HttpGet("check-email")]
//    public async Task<IActionResult> CheckEmailExists([FromQuery] string email)
//    {
//        if (string.IsNullOrEmpty(email))
//            return BadRequest(new { Exists = false, Message = "Email is required." });

//        var user = await _userManager.FindByEmailAsync(email.Trim().ToLower());

//        if (user != null)
//            return Ok(new { Exists = true, Message = "Email already exists." });

//        return Ok(new { Exists = false, Message = "Email is available." });
//    }