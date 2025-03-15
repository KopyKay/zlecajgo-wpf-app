namespace ZlecajGoApi.Exceptions;

public class EmailAlreadyInUseException(string email) : Exception($"Adres email \"{email}\" jest już zarejestrowany!");