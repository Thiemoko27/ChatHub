using ChatHub.Models;

public interface IAuthService {
    string GenerateJwtToken(User user);
    bool ValidateStudentCredential(string userName, string password, out User user);
}