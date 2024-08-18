using ChatHub.Data;
using ChatHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatHub.Controllers;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase {
    private readonly UserDataBaseContext _userContext;

    public UserController(UserDataBaseContext userContext) {
        _userContext = userContext;
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id) {
        var user = _userContext.Users.Find(id);

        if(user == null)    return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] User user) {
        if(user == null)    return BadRequest("user data can't be null");

        if(user.Password == null)   return BadRequest("Password also can't be null");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        _userContext.Users.Add(user);
        _userContext.SaveChanges();

        return CreatedAtAction(nameof(GetUserById), new {id = user.Id}, user);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser([FromBody] User updateUser, int id) {
        var user = _userContext.Users.Find(id);

        if(user == null)    return BadRequest("user data can't be null");

        if(user.Password == null)   return BadRequest("Password also can't be null");

        user.UserName = updateUser.UserName;
        user.Email = updateUser.Email;
        user.Password = BCrypt.Net.BCrypt.HashPassword(updateUser.Password);

        _userContext.Users.Update(user);
        _userContext.SaveChanges();

        return NoContent();
    }
}