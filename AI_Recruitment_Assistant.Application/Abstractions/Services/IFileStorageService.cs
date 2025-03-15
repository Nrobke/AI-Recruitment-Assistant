
using Microsoft.AspNetCore.Http;

namespace AI_Recruitment_Assistant.Application.Abstractions.Services;

public interface IFileStorageService
{
    Task<string> SaveFile(IFormFile file);
}
