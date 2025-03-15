namespace ZlecajGoApi.Exceptions;

public class UsernameAlreadyInUseException(string userName) : Exception($"Nazwa użytkownika \"{userName}\" jest już zarejestrowana!");