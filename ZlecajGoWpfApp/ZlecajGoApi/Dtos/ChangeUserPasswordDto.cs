namespace ZlecajGoApi.Dtos;

public class ChangeUserPasswordDto
{
    public string NewPassword { get; set; } = null!;
    public string ConfirmNewPassword { get; set; } = null!;
    public string CurrentPassword { get; set; } = null!;
}