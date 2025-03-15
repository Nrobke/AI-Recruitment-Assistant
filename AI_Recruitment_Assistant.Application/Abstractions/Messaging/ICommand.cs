using AI_Recruitment_Assistant.Domain.Shared;
using MediatR;

namespace AI_Recruitment_Assistant.Application.Abstractions.Messaging;

public interface ITransactionalCommand : ICommand;
public interface ITransactionalCommand<TResponse> : ICommand<TResponse>, ITransactionalCommand;
public interface ICommand : IRequest<Result>;
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;

