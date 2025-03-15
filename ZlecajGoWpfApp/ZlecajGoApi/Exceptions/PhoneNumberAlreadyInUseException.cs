namespace ZlecajGoApi.Exceptions;

public class PhoneNumberAlreadyInUseException(string phoneNumber) : Exception($"Numer telefonu \"{phoneNumber}\" jest już zarejestrowany!");