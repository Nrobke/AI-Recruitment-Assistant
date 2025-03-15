
using AI_Recruitment_Assistant.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;

namespace AI_Recruitment_Assistant.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    public async Task<string> SaveFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Invalid file");
        }

        // Get the Documents folder path
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var uploadsPath = Path.Combine(documentsPath, "UploadedCVs");

        // Create directory if it doesn't exist
        Directory.CreateDirectory(uploadsPath);

        // Generate unique filename
        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        // Save the file
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return filePath;
    }
}
