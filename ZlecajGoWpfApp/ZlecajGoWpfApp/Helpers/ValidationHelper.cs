using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ZlecajGoWpfApp.Helpers;

public static partial class ValidationHelper
{
    [GeneratedRegex(@"^\d{0,2}(-\d{0,3})?$")]
    public static partial Regex ValidPostalCodePreviewInputRegex();
    
    [GeneratedRegex(@"^(?!0)\d{1,7}(\,\d{0,2})?$")]
    public static partial Regex ValidPricePreviewInputRegex();
    
    public static class ErrorMessage
    {
        public const string FieldIsRequired = "Pole jest wymagane!";
        public const string FieldContainsIllegalCharacters = "Pole zawiera niedozwolone znaki!";
        public const string FieldIsTooShort = "Pole wymaga więcej znaków!";
        public const string FieldIncorrectFormat = "Pole ma niepoprawny format!";
    
        public const string IncorrectEmail = "Niepoprawny adres email!";
        public const string PasswordIsTooShort = "Hasło jest za krótkie!";
        public const string PasswordDoesNotMeetRequirements = "Hasło nie spełnia wymagań!";
        public const string PasswordsDoNotMatch = "Hasła nie są takie same!";
        public const string IncorrectPhoneNumber = "Niepoprawny numer telefonu!";
    }

    public static class RegularExpression
    {
        public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*\W).{6,}$";
        public const string FirstName = @"^\p{L}+$";
        public const string LastName = @"^\p{L}+$";
        public const string UserName = "^[A-Za-z0-9_]+$";
        public const string PolishPhoneNumber = @"^\+48\s?[4-9]\d{8}$";
        public const string OfferTitle = @"^[a-zA-ZĄĆĘŁŃÓŚŹŻąćęłńóśźż0-9\s\p{P}]+$";
        public const string OfferDescription = @"^[a-zA-ZĄĆĘŁŃÓŚŹŻąćęłńóśźż0-9\s\p{P}\p{S}]+$";
        public const string PostalCode = @"^\d{2}-\d{3}$";
        public const string StreetName = @"^[a-zA-ZĄĆĘŁŃÓŚŹŻąćęłńóśźż0-9\s\-\']+$";
        public const string StreetNumber = @"^[a-zA-Z0-9\/]+$";
    }

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
                return new ValidationResult(ValidationHelper.ErrorMessage.PasswordsDoNotMatch);
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