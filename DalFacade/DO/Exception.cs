namespace DO;

[Serializable]
public class DalNotExistException : Exception
{
    public DalNotExistException(string? message) : base(message) { }
}

public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}

public class DalDeletionImpossibleException : Exception
{
    public DalDeletionImpossibleException(string? message) : base(message) { }
}
