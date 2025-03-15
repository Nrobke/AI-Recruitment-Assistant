
namespace AI_Recruitment_Assistant.Domain.Entities;

public class Interview : BaseEntity
{
    public int Id { get; set; }
    public DateTime? InterviewDate { get; set; }
    public string MeetingLink { get; set; }
    public bool FollowUpSent { get; set; }
    public int JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; }
}
