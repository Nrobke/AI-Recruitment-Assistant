
namespace AI_Recruitment_Assistant.Domain.Entities;

public class JobPosting : BaseEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string RequiredSkills { get; set; }
    public float MatchThreshold { get; set; } 
    public DateTime ExpiryDate { get; set; }
    public int CreatedByUserId { get; set; }
    public int TopCandidatesCount { get; set; }
    public bool IsProcessed { get; set; }
    public User CreatedByUser { get; set; }
    public ICollection<JobApplication> Applications { get; set; }
}
