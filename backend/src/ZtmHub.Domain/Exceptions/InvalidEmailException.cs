namespace ZtmHub.Domain.Exceptions;

public class InvalidEmailException(string message) : DomainException(message);