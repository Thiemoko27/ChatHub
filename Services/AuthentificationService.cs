using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ChatHub.Models;
using ChatHub.Data;

namespace ChatHub.Services;

public class AuthService {
    private readonly UserDataBaseContext _userContext = null!;
    private readonly IConfiguration _configuration = null!;

    public AuthService(IConfiguration configuration, UserDataBaseContext userContext) {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    //Création d'un service d'où va puiser les deux models pour le controlleur d'authentification

    //instance de GenerateJwtToken pour User
    public string GenerateJwtToken(User user) {
        if(user == null) throw new ArgumentNullException(nameof(user));
        return GenerateJwtToken(user.UserName, user.Id.ToString());
    }


    public bool ValidateStudentCredential(string userName, string password, out User user) {
        user = _userContext.Users.SingleOrDefault(u => u.UserName == userName)
                ?? throw new ArgumentNullException("user can't be null");

        if(user == null) return false;

        return BCrypt.Net.BCrypt.Verify(password, user.Password);
    }

    //Création du système d'authentification de json web token
    private string GenerateJwtToken(string? userName, string userId) {
        if(string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));

        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        if(string.IsNullOrEmpty(jwtKey)) throw new ArgumentNullException(nameof(jwtKey));
        if(string.IsNullOrEmpty(jwtIssuer)) throw new ArgumentException(nameof(jwtIssuer));
        if(string.IsNullOrEmpty(jwtAudience)) throw new ArgumentNullException(nameof(jwtAudience));

        if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience)) {
                throw new ApplicationException("JWT configuration is missing or invalid.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var Claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", userId)
        };

        //Création du token d'une durée de vie de 2h
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: Claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}