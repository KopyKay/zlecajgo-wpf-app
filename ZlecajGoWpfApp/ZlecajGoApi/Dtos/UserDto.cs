namespace ZlecajGoApi.Dtos;

public class UserDto
{
    public string Id { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string? FullName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? UserName { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool IsProfileCompleted { get; set; } = false;

    public override string ToString()
    {
        return $"Id: {Id}\nImię i nazwisko: {FullName}\nData urodzenia: {BirthDate}\n" +
               $"Nazwa użytkownika: {UserName}\nEmail: {Email}\nNumer telefonu: {PhoneNumber}\n";
    }
}