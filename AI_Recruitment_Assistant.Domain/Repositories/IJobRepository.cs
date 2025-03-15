
using AI_Recruitment_Assistant.Domain.Entities;

namespace AI_Recruitment_Assistant.Domain.Repositories;

public interface IJobRepository
{
    Task<List<JobPosting>> GetExpiredJobPostingsAsync(CancellationToken cancellationToken);
}
