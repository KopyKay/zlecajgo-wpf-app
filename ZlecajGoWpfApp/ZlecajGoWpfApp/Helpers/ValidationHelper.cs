using System.Text.RegularExpressions;
using ZlecajGoApi.Dtos;

namespace ZlecajGoWpfApp.Helpers;

public static partial class ValidationHelper
{
    [GeneratedRegex("^(?=.*[0-9])(?=.*[A-Z]).{6,}$")]
    private static partial Regex ValidPassword();
    
    [GeneratedRegex("^(?=.*[@])(?=.*[.]).+$")]
    private static partial Regex ValidEmail();

    public static void LogInValidation(LogInDto logInDto)
    {
        CheckIfInputsAreEmpty(logInDto.Email, logInDto.Password);
        Email(logInDto.Email);
    }
    
    public static void SignUpValidation(SignUpDto signUpDto)
    {
        CheckIfInputsAreEmpty(signUpDto.Email, signUpDto.Password, signUpDto.ConfirmPassword);
        Email(signUpDto.Email);
        Password(signUpDto.Password, signUpDto.ConfirmPassword);
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

    private static void CheckIfInputsAreEmpty(params string[] inputs)
    {
        if (inputs.Any(string.IsNullOrWhiteSpace))
        {
            ThrowException("Wszytkie pola muszą być wypełnione!");
        }
    }
    
    private static void ThrowException(string message) => throw new ArgumentException(message);
}