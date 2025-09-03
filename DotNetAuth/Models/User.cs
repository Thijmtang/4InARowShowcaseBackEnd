namespace DotNetAuth.Models;

public record User(
    Guid Id,
    string Username,
    string Email,
    IList<string> Roles
    );