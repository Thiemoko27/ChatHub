using ChatHub.Data;
using ChatHub.Models;
using ChatHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatHub.Controllers;

[ApiController]
[Route("[controller]")]

public class UserAuthController : ControllerBase {
    private readonly UserDataBaseContext _userContext;
    private readonly AuthService _authService;

    public UserAuthController(UserDataBaseContext userContext, AuthService authService) {
        _userContext = userContext;
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginRequest loginRequest) {
        var user = _userContext.Users.SingleOrDefault(u => u.UserName == loginRequest.UserName);

        if(user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password)) {
            return Unauthorized(new {message = "Invalid userName or password"});
        }

        var token = _authService.GenerateJwtToken(user);
        return Ok(new {token});
    }
}