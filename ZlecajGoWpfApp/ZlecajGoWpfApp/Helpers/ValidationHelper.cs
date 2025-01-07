using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ZlecajGoWpfApp.Helpers;

public static partial class ValidationHelper
{
    public const string FieldIsRequiredMessage = "Pole jest wymagane!";
    public const string FieldContainsIllegalCharactersMessage = "Pole zawiera niedozwolone znaki!";
    public const string FieldTooShortMessage = "Pole wymaga więcej znaków!";
    public const string FieldIncorrectFormatMessage = "Pole ma niepoprawny format!";
    
    public const string IncorrectEmailMessage = "Niepoprawny adres email!";
    public const string PasswordIsTooShortMessage = "Hasło jest za krótkie!";
    public const string PasswordDoesNotMeetRequirementsMessage = "Hasło nie spełnia wymagań!";
    public const string PasswordsDoNotMatchMessage = "Hasła nie są takie same!";
    public const string IncorrectPhoneNumberMessage = "Niepoprawny numer telefonu!";

    public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*\W).{6,}$";
    public const string FirstNameRegex = @"^\p{L}+$";
    public const string LastNameRegex = @"^\p{L}+$";
    public const string UserNameRegex = "^[A-Za-z0-9_]+$";
    public const string PhoneNumberRegex = @"^\+48\s?[4-9]\d{8}$";
    public const string TitleRegex = @"^[a-zA-ZĄĆĘŁŃÓŚŹŻąćęłńóśźż0-9\s\p{P}]+$";
    public const string DescriptionRegex = @"^[a-zA-ZĄĆĘŁŃÓŚŹŻąćęłńóśźż0-9\s\p{P}\p{S}]+$";
    public const string PostalCodeRegex = @"^\d{2}-\d{3}$";
    public const string StreetNameRegex = @"^[a-zA-ZĄĆĘŁŃÓŚŹŻąćęłńóśźż0-9\s\-\']+$";
    public const string StreetNumberRegex = @"^[a-zA-Z0-9\/]+$";
    
    [GeneratedRegex(@"^\d{0,2}(-\d{0,3})?$")]
    public static partial Regex ValidPostalCodePreviewInputRegex();
    
    [GeneratedRegex(@"^(?!0)\d{1,7}(\,\d{0,2})?$")]
    public static partial Regex ValidPricePreviewInputRegex();

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class ComparePasswordsAttribute(string comparisonProperty) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = value as string;
            var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(comparisonProperty);

            if (comparisonPropertyInfo is null)
            {
                return new ValidationResult($"'{comparisonProperty}' not found.");
            }

            var comparisonValue = comparisonPropertyInfo.GetValue(validationContext.ObjectInstance) as string;

            if (currentValue != comparisonValue)
            {
                return new ValidationResult(PasswordsDoNotMatchMessage);
            }

            return ValidationResult.Success;
        }
    }
    
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class MinimumAgeAttribute(int minimumAge) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            DateOnly? birthDate = null;

            try
            {
                switch (value)
                {
                    case string birthDateString:
                    {
                        if (DateOnly.TryParseExact(birthDateString, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                        {
                            birthDate = parsedDate;
                        }

                        break;
                    }
                    case DateTime birthDateTime:
                        birthDate = DateOnly.FromDateTime(birthDateTime);
                        break;
                    case DateOnly birthDateOnly:
                        birthDate = birthDateOnly;
                        break;
                    default:
                        return new ValidationResult("Nieprawidłowa data urodzenia!");

                }
            }
            catch (Exception)
            {
                return new ValidationResult("Nieprawidłowa data urodzenia!");
            }
            
            var age = DateOnly.FromDateTime(DateTime.Now).Year - birthDate!.Value.Year;
            
            if (birthDate.Value.AddYears(age) > DateOnly.FromDateTime(DateTime.Now))
            {
                age--;
            }

            if (age < minimumAge)
            {
                return new ValidationResult($"Użytkownik musi mieć co najmniej {minimumAge} lat!");
            }

            return ValidationResult.Success;
        }
    }
}