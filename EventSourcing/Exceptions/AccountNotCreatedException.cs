namespace EventSourcing.Exceptions;

public class AccountNotCreatedException(string message) : InvalidOperationException(message);
