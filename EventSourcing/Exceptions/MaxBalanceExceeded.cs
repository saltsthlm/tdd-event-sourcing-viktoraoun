namespace EventSourcing.Exceptions;

public class MaxBalanceExceeded(string message) : InvalidOperationException(message);
