using AI_Recruitment_Assistant.Domain.Entities;
using AI_Recruitment_Assistant.Infrastructure.Persistence;
using Bogus;
using MerchantAppBackend.Infrastructure.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AI_Recruitment_Assistant.Infrastructure.Seed;

public class DatabaseSeeder(ApplicationDbContext context, UserManager<User> userManager) : IDatabaseSeeder
{
    public async Task Seed()
    {
        if (await context.Database.CanConnectAsync())
        {
            await SeedSystemConstants();
            await SeedUsers();
            await SeedJobPostings(context);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedUsers()
    {
        if (!await userManager.Users.AnyAsync())
        {
            var adminType = await context.SystemConstants
                .FirstAsync(sc => sc.Type == "UserType" && sc.Name == "Admin");

            var admin = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                UserTypeId =  adminType.Id
            };

            await userManager.CreateAsync(admin, "Admin@123");
            await context.SaveChangesAsync();

        }
    }
    private async Task SeedSystemConstants()
    {
        if (!await context.SystemConstants.AnyAsync())
        {
            var constants = new List<SystemConstant>
            {
                new() { Type = "UserType", Name = "Admin", IsActive = true },
                new() { Type = "UserType", Name = "Candidate", IsActive = true },
                new() { Type = "ApplicationStatus", Name = "Pending", IsActive = true },
                new() { Type = "ApplicationStatus", Name = "Approved", IsActive = true },
                new() { Type = "ApplicationStatus", Name = "Rejected", IsActive = true }
            };

            await context.SystemConstants.AddRangeAsync(constants);
            await context.SaveChangesAsync();

        }
    }
    private static async Task SeedJobPostings(ApplicationDbContext context)
    {
        if (!await context.JobPostings.AnyAsync())
        {
            var industries = new[]
            {
            new { Name = "Technology", Count = 10 },
            new { Name = "Healthcare", Count = 5 },
            new { Name = "Finance", Count = 5 },
            new { Name = "Education", Count = 5 },
            new { Name = "Engineering", Count = 5 }
        };

            var allJobs = new List<JobPosting>();

            foreach (var industry in industries)
            {
                var industryJobs = industry.Name switch
                {
                    "Technology" => GenerateTechJobs(industry.Count),
                    "Healthcare" => GenerateHealthcareJobs(industry.Count),
                    "Finance" => GenerateFinanceJobs(industry.Count),
                    "Education" => GenerateEducationJobs(industry.Count),
                    "Engineering" => GenerateEngineeringJobs(industry.Count),
                    _ => throw new ArgumentException("Unknown industry")
                };

                allJobs.AddRange(industryJobs);
            }

            await context.JobPostings.AddRangeAsync(allJobs);
            await context.SaveChangesAsync();
        }
    }

    private static List<JobPosting> GenerateTechJobs(int count)
    {
        var techTitles = new[]
        {
        "Senior Software Engineer", "Cloud Architect", "DevOps Specialist",
        "AI Research Scientist", "Cybersecurity Analyst", "Data Engineer",
        "Mobile App Developer", "UX/UI Designer", "QA Automation Engineer",
        "Blockchain Developer"
    };

        var techSkills = new[]
        {
        "C#, .NET Core, Azure, SQL",
        "AWS, Terraform, Kubernetes",
        "Python, TensorFlow, PyTorch",
        "Java, Spring Boot, Microservices",
        "React, TypeScript, Redux",
        "Swift, iOS Development, Xcode",
        "Figma, Sketch, Adobe XD",
        "Selenium, JMeter, TestRail"
    };

        return CreateIndustryJobs(count, techTitles, techSkills, "Technology");
    }

    private static List<JobPosting> GenerateHealthcareJobs(int count)
    {
        var titles = new[]
        {
        "Registered Nurse", "Physical Therapist", "Medical Laboratory Technician",
        "Pharmacy Manager", "Clinical Research Coordinator"
    };

        var skills = new[]
        {
        "CPR Certification, Patient Care",
        "Rehabilitation Therapy, Anatomy Knowledge",
        "Lab Equipment Operation, Bio-safety Protocols",
        "Pharmaceutical Management, FDA Regulations",
        "Clinical Trial Management, GCP Compliance"
    };

        return CreateIndustryJobs(count, titles, skills, "Healthcare");
    }

    private static List<JobPosting> GenerateFinanceJobs(int count)
    {
        var titles = new[]
        {
        "Financial Analyst", "Investment Banker", "Risk Manager",
        "Certified Accountant", "Audit Specialist"
    };

        var skills = new[]
        {
        "Financial Modeling, Excel Advanced",
        "M&A Deals, Capital Markets",
        "Risk Assessment, Compliance",
        "GAAP, Tax Preparation",
        "Internal Auditing, SOX Compliance"
    };

        return CreateIndustryJobs(count, titles, skills, "Finance");
    }

    private static List<JobPosting> GenerateEducationJobs(int count)
    {
        var titles = new[]
        {
        "High School Teacher", "University Professor",
        "Curriculum Developer", "Education Consultant",
        "Special Needs Educator"
    };

        var skills = new[]
        {
        "Lesson Planning, Classroom Management",
        "Academic Research, Thesis Supervision",
        "Pedagogy Development, LMS Platforms",
        "Education Policy, Accreditation",
        "Individualized Education Plans (IEPs)"
    };

        return CreateIndustryJobs(count, titles, skills, "Education");
    }

    private static List<JobPosting> GenerateEngineeringJobs(int count)
    {
        var titles = new[]
        {
        "Civil Engineer", "Mechanical Engineer",
        "Electrical Engineer", "Chemical Engineer",
        "Environmental Engineer"
    };

        var skills = new[]
        {
        "AutoCAD, Structural Analysis",
        "SolidWorks, Thermodynamics",
        "Circuit Design, PLC Programming",
        "Process Engineering, HYSYS",
        "Environmental Impact Assessment"
    };

        return CreateIndustryJobs(count, titles, skills, "Engineering");
    }

    private static List<JobPosting> CreateIndustryJobs(int count, string[] titles, string[] skills, string industry)
    {
        var faker = new Faker<JobPosting>()
            .RuleFor(j => j.Title, f => $"{f.PickRandom(titles)} ({industry})")
            .RuleFor(j => j.Description, f => GenerateIndustryDescription(f, industry))
            .RuleFor(j => j.RequiredSkills, f => f.PickRandom(skills))
            .RuleFor(j => j.MatchThreshold, f => f.Random.Float(0.65f, 0.85f))
            .RuleFor(j => j.ExpiryDate, f => f.Date.Future(60, DateTime.UtcNow))
            .RuleFor(j => j.CreatedByUserId, 1)
            .RuleFor(j => j.TopCandidatesCount, f => f.Random.Int(3, 10))
            .RuleFor(j => j.IsProcessed, false);

        return faker.Generate(count);
    }

    private static string GenerateIndustryDescription(Faker f, string industry)
    {
        return industry switch
        {
            "Technology" => $"We're seeking a skilled {f.PickRandom(new[] { "developer", "engineer", "specialist" })} " +
                           "to join our tech team. Requirements include:\n" +
                           $"- {f.PickRandom(new[] { "3+ years experience", "Relevant certifications" })}\n" +
                           $"- {f.PickRandom(new[] { "Strong problem-solving skills", "Agile methodology experience" })}",

            "Healthcare" => $"Looking for a qualified {f.PickRandom(new[] { "medical professional", "healthcare specialist" })} " +
                           "to join our healthcare team. Requirements:\n" +
                           $"- {f.PickRandom(new[] { "Valid license", "Patient care experience" })}\n" +
                           $"- {f.PickRandom(new[] { "EMR system knowledge", "CPR certification" })}",

            "Finance" => $"Seeking an experienced {f.PickRandom(new[] { "finance professional", "analyst" })} " +
                        "for our financial department. Requirements include:\n" +
                        $"- {f.PickRandom(new[] { "CFA/CPA certification", "Financial modeling expertise" })}\n" +
                        $"- {f.PickRandom(new[] { "Risk assessment skills", "Regulatory compliance knowledge" })}",

            "Education" => $"We need a dedicated {f.PickRandom(new[] { "educator", "teacher" })} " +
                          "to join our educational institution. Requirements:\n" +
                          $"- {f.PickRandom(new[] { "Teaching certification", "Curriculum development experience" })}\n" +
                          $"- {f.PickRandom(new[] { "Classroom management skills", "Student assessment expertise" })}",

            "Engineering" => $"Looking for a {f.PickRandom(new[] { "skilled engineer", "engineering specialist" })} " +
                            "to join our engineering team. Requirements include:\n" +
                            $"- {f.PickRandom(new[] { "Professional Engineer license", "CAD software expertise" })}\n" +
                            $"- {f.PickRandom(new[] { "Project management experience", "Technical documentation skills" })}",

            _ => "Job description"
        };
    }
}