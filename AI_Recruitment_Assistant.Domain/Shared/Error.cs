namespace AI_Recruitment_Assistant.Domain.Shared;

public record Error
{
    public static readonly Error None = new(ErrorCode.None, "No error occurred.", ErrorType.Failure);
    public static readonly Error NullValue = new(ErrorCode.GeneralNull, "Null value was provided.", ErrorType.Failure);
    public static readonly Error NotFoundInstance = new(ErrorCode.NotFound, "The requested entity was not found.", ErrorType.NotFound);
    public static readonly Error ConflictInstance = new(ErrorCode.Conflict, "A conflict occurred with the existing data.", ErrorType.Conflict);
    public static readonly Error ValidationFailure = new(ErrorCode.ValidationFailure, "One or more validation errors occurred.", ErrorType.Failure);
    public static readonly Error Unexpected = new(ErrorCode.UnexpectedError, "An unexpected error occurred", ErrorType.Problem);
    public static readonly Error Unauthenticated = new(ErrorCode.Unauthenticated, "User not authenticated", ErrorType.Failure);

    public Error(ErrorCode code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public ErrorCode Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public static Error Failure(ErrorCode code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error NotFound(ErrorCode code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Conflict(ErrorCode code, string description) =>
        new(code, description, ErrorType.Conflict);
    public static Error UnexpectedError(string description) =>
       new(ErrorCode.UnexpectedError, description, ErrorType.Problem);
}
