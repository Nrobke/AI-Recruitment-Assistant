
namespace AI_Recruitment_Assistant.Domain.Entities;

public class JobApplication : BaseEntity
{
    public int Id { get; set; }
    public string CvFilePath { get; set; }
    public string ParsedSkills { get; set; }
    public float MatchScore { get; set; }
    public DateTime AppliedDate { get; set; }
    public int CandidateId { get; set; }
    public int JobPostingId { get; set; }
    public int StatusId { get; set; }
    public User Candidate { get; set; }
    public SystemConstant Status { get; set; }
    public JobPosting JobPosting { get; set; }
    public ICollection<Interview> Interviews { get; set; }
}
