using AI_Recruitment_Assistant.Domain.Shared;
using MediatR;

namespace AI_Recruitment_Assistant.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
