using AI_Recruitment_Assistant.Domain.Shared;
using MediatR;

namespace AI_Recruitment_Assistant.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
