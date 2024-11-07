using System.Text.RegularExpressions;
using ZlecajGoApi.Dtos;

namespace ZlecajGoWpfApp.Helpers;

public static partial class ValidationHelper
{
    [GeneratedRegex("^(?=.*[@])(?=.*[.]).+$")]
    private static partial Regex ValidEmail();
    
    [GeneratedRegex("^(?=.*[0-9])(?=.*[A-Z]).{6,}$")]
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
        Email(dto.Email);
    }
    
    public static void SignUpValidation(SignUpDto dto)
    {
        CheckIfInputsAreEmpty(dto.Email, dto.Password, dto.ConfirmPassword);
        Email(dto.Email);
        Password(dto.Password, dto.ConfirmPassword);
    }
    
    public static void UpdateUserCredentialsValidation(UpdateUserCredentialsDto dto)
    {
        CheckIfInputsAreEmpty(dto.FullName, dto.BirthDate, dto.UserName, dto.PhoneNumber);
        FullName(dto.FullName!);
        BirthDate((DateOnly)dto.BirthDate!);
        UserName(dto.UserName!);
        PhoneNumber(dto.PhoneNumber!);
    }
    
    private static void Email(string email)
    {
        if (!ValidEmail().IsMatch(email))
        {
            ThrowException("Niepoprawny adres email!");
        }
    }
    
    private static void Password(string password, string confirmPassword)
    {
        if (!ValidPassword().IsMatch(password))
        {
            ThrowException("Hasło musi mieć co najmniej 6 znaków, jedną cyfrę i jedną dużą literę!");
        }
        
        if (!string.Equals(password, confirmPassword))
        {
            ThrowException("Hasła nie są takie same!");
        }
    }
    
    private static void FullName(string fullName)
    {
        if (!ValidFullName().IsMatch(fullName))
        {
            ThrowException("Niepoprawne imię lub nazwisko!");
        }
    }
    
    private static void BirthDate(DateOnly birthDate)
    {
        if (birthDate.AddYears(18) > DateOnly.FromDateTime(DateTime.Now))
        {
            ThrowException("Użytkownik musi mieć co najmniej 18 lat!");
        }
    }
    
    private static void UserName(string userName)
    {
        if (!ValidUserName().IsMatch(userName))
        {
            ThrowException("Nazwa użytkownika może składać się tylko z liter, cyfr oraz znaku '_' w długości od 3 do 30 znaków!");
        }
    }
    
    private static void PhoneNumber(string phoneNumber)
    {
        if (!ValidPhoneNumber().IsMatch(phoneNumber))
        {
            ThrowException("Niepoprawny numer telefonu!");
        }
    }
    
    private static void ProfilePictureUrl(string profilePictureUrl)
    {
        if (!ValidProfilePictureUrl().IsMatch(profilePictureUrl))
        {
            ThrowException("Niepoprawny adres URL zdjęcia profilowego!");
        }
    }
    
    private static void CheckIfInputsAreEmpty(params object?[] inputs)
    {
        var emptyInputsMessage = "Wszytkie pola muszą być wypełnione!";
        
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