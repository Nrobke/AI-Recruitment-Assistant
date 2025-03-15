namespace AI_Recruitment_Assistant.Domain.Entities;

public class SystemConstant : BaseEntity
{
    public int Id { get; set; }
    public string Type { get; set; } = default!;
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public ICollection<User> UserTypesNavigation { get; set; }
    public ICollection<JobApplication> StatusesNavigation { get; set; }
}
