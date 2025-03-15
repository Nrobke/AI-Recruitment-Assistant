
using Microsoft.AspNetCore.Identity;

namespace AI_Recruitment_Assistant.Domain.Entities;

public class User : IdentityUser<int>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UserTypeId { get; set; }
    public SystemConstant UserType { get; set; }
    public ICollection<JobPosting> CreatedJobPostings { get; set; }
}
