
namespace AI_Recruitment_Assistant.Application.DTOs.Responses;

public record CVParseResult(
    List<string> Skills,
    string Education,
    string Experience,
    string Summary
);
