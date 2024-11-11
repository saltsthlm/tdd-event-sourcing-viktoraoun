namespace EventSourcing.Exceptions;

public class EventTypeNotSupportedException(string message) : NotSupportedException(message);
