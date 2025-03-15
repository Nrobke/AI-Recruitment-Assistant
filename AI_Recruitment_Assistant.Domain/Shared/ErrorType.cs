namespace AI_Recruitment_Assistant.Domain.Shared;

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Problem = 4
}

public enum ErrorCode
{
    None,
    GeneralNull,
    NotFound,
    Conflict,
    ValidationFailure,
    UnexpectedError,
    Unauthenticated,
    Unauthorized
}
