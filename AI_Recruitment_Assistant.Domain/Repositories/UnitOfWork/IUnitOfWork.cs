namespace AI_Recruitment_Assistant.Domain.Repositories.UnitOfWork;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
