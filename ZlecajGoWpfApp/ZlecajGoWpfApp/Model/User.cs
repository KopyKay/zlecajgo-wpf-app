namespace ZlecajGoWpfApp.Model;

public class User
{
    public string Id { get; set; } = null!;
    public string? FullName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? UserName { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
}