namespace ZlecajGoApi.Dtos;

public class UpdateUserCredentialsDto
{
    public string? FullName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
}