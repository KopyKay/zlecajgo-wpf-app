using System.Text.RegularExpressions;
using ZlecajGoApi.Dtos;

namespace ZlecajGoWpfApp.Helpers;

public static partial class ValidationHelper
{
    [GeneratedRegex("^[a-zA-Z0-9_-]+@[a-zA-Z]+\\.[a-zA-Z]{2,}$")]
    private static partial Regex ValidEmail();
    
    [GeneratedRegex("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*(),.?\":{}|<>]).{6,}$")]
    private static partial Regex ValidPassword();
    
    [GeneratedRegex("^[A-Za-z]{3,100} [A-Za-z]{3,100}$")]
    private static partial Regex ValidFullName();

    [GeneratedRegex("^[A-Za-z0-9_]{3,30}$")]
    private static partial Regex ValidUserName();
    
    [GeneratedRegex(@"^\+48[4-9]\d{8}$")]
    private static partial Regex ValidPhoneNumber();
    
    [GeneratedRegex(@"^(http:\/\/|https:\/\/).+$")]
    private static partial Regex ValidProfilePictureUrl();
    
    public static void LogInValidation(LogInDto dto)
    {
        CheckIfInputsAreEmpty(dto.Email, dto.Password);
        CheckEmail(dto.Email);
    }
    
    public static void SignUpValidation(SignUpDto dto)
    {
        CheckIfInputsAreEmpty(dto.Email, dto.Password, dto.ConfirmPassword);
        CheckEmail(dto.Email);
        CheckPassword(dto.Password, dto.ConfirmPassword);
    }
    
    public static void UpdateUserCredentialsValidation(UpdateUserCredentialsDto dto)
    {
        CheckIfInputsAreEmpty(dto.FullName, dto.BirthDate, dto.UserName, dto.PhoneNumber);
        CheckFullName(dto.FullName!);
        CheckBirthDate((DateOnly)dto.BirthDate!);
        CheckUserName(dto.UserName!);
        CheckPhoneNumber(dto.PhoneNumber!);
    }
    
    private static void CheckEmail(string email)
    {
        if (!ValidEmail().IsMatch(email))
        {
            ThrowException("Niepoprawny adres email!");
        }
    }
    
    private static void CheckPassword(string password, string confirmPassword)
    {
        if (!ValidPassword().IsMatch(password))
        {
            ThrowException("Nieprawidłowe hasło!");
        }
        
        if (!string.Equals(password, confirmPassword))
        {
            ThrowException("Hasła nie są takie same!");
        }
    }
    
    private static void CheckFullName(string fullName)
    {
        if (!ValidFullName().IsMatch(fullName))
        {
            ThrowException("Niepoprawne imię lub nazwisko!");
        }
    }
    
    private static void CheckUserName(string userName)
    {
        if (!ValidUserName().IsMatch(userName))
        {
            ThrowException("Nazwa użytkownika może zawierać (A-Z, a-z, 0-9, _), od 3 do 30 znaków!");
        }
    }
    
    private static void CheckBirthDate(DateOnly birthDate)
    {
        if (birthDate.AddYears(18) > DateOnly.FromDateTime(DateTime.Now))
        {
            ThrowException("Użytkownik musi mieć co najmniej 18 lat!");
        }
    }
    
    private static void CheckPhoneNumber(string phoneNumber)
    {
        if (!ValidPhoneNumber().IsMatch(phoneNumber))
        {
            ThrowException("Niepoprawny numer telefonu!");
        }
    }
    
    private static void CheckProfilePictureUrl(string profilePictureUrl)
    {
        if (!ValidProfilePictureUrl().IsMatch(profilePictureUrl))
        {
            ThrowException("Niepoprawny adres URL zdjęcia profilowego!");
        }
    }
    
    private static void CheckIfInputsAreEmpty(params object?[] inputs)
    {
        const string emptyInputsMessage = "Wszystkie pola muszą być wypełnione!";
        
        if (inputs.Length == 0)
        {
            ThrowException(emptyInputsMessage);
        }
        
        foreach (var input in inputs)
        {
            switch (input)
            {
                case string str when string.IsNullOrWhiteSpace(str):
                case DateOnly date when date == default:
                case null:
                    ThrowException(emptyInputsMessage);
                    break;
            }
        }
    }
    
    private static void ThrowException(string message) => throw new ArgumentException(message);
}