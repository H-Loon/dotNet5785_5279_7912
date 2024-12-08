namespace BO;

[Serializable]
public class BlNotExistException : Exception
{
    public BlNotExistException(string? message) : base(message) { }
    public BlNotExistException(string? message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string? message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlDeletionImpossibleException : Exception
{
    public BlDeletionImpossibleException(string? message) : base(message) { }
    public BlDeletionImpossibleException(string? message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlXMLFileLoadCreateException : Exception
{
    public BlXMLFileLoadCreateException(string? message) : base(message) { }
    public BlXMLFileLoadCreateException(string? message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlAddressNotValidException : Exception
{
    public BlAddressNotValidException(string? message) : base(message) { }
}

[Serializable]
public class BlCoordinatesNotFoundException : Exception
{
    public BlCoordinatesNotFoundException(string? message) : base(message) { }
}

[Serializable]
public class BlIncorrectPasswordException : Exception
{
    public BlIncorrectPasswordException(string? message) : base(message) { }
}