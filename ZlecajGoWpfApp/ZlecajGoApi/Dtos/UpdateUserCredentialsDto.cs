namespace ZlecajGoApi.Dtos;

public class UpdateUserCredentialsDto
{
    public string? FullName { get; set; } = null!;
    public DateOnly? BirthDate { get; set; }
    public string? UserName { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
}