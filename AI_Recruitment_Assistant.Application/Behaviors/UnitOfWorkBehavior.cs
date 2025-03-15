
using System.Transactions;
using AI_Recruitment_Assistant.Application.Abstractions.Messaging;
using AI_Recruitment_Assistant.Domain.Repositories.UnitOfWork;
using MediatR;

namespace AI_Recruitment_Assistant.Application.Behaviors;

public sealed class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork) : IPipelineBehavior<TRequest, TResponse> where TRequest : ITransactionalCommand
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {

        using var transactionScope = new TransactionScope();
        var response = await next();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        transactionScope.Complete();

        return response;
    }

}

