

using AI_Recruitment_Assistant.Domain.Entities;
using AI_Recruitment_Assistant.Domain.Repositories;
using AI_Recruitment_Assistant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AI_Recruitment_Assistant.Infrastructure.Repositories;

public class JobRepository(ApplicationDbContext context) : IJobRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<List<JobPosting>> GetExpiredJobPostingsAsync(CancellationToken cancellationToken)
    {
        try
		{
            return await _context.JobPostings
               .Where(j => j.ExpiryDate < DateTime.UtcNow && !j.IsProcessed)
               .Include(j => j.Applications)
                   .ThenInclude(a => a.Candidate)
               .Include(j => j.Applications)
                   .ThenInclude(a => a.JobPosting)
               .ToListAsync(cancellationToken);
        }
		catch (Exception)
		{

            throw;
        }
    }
}
