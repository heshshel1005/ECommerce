using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace ECommerce.OrganizationSignup;

/// <summary>
/// Stores organization signup logos under App_Data/OrganizationSignupLogos on the host.
/// </summary>
public class OrganizationSignupLogoFileStorage : IOrganizationSignupLogoStorage
{
    private readonly IWebHostEnvironment _env;
    private static readonly Dictionary<string, string> ContentTypesByExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".jpe", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".webp", "image/webp" },
        { ".svg", "image/svg+xml" }
    };

    public OrganizationSignupLogoFileStorage(IWebHostEnvironment env)
    {
        _env = env;
    }

    private string GetRootPath()
    {
        var path = Path.Combine(_env.ContentRootPath, "App_Data", "OrganizationSignupLogos");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    public async Task<string> SaveAsync(Stream stream, string fileName, Guid requestId, CancellationToken cancellationToken = default)
    {
        var root = GetRootPath();
        var requestDir = Path.Combine(root, requestId.ToString("N"));
        if (!Directory.Exists(requestDir))
            Directory.CreateDirectory(requestDir);

        var ext = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(ext))
            ext = ".bin";
        var safeName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(requestDir, safeName);
        var relativePath = $"App_Data/OrganizationSignupLogos/{requestId:N}/{safeName}";

        await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
        {
            await stream.CopyToAsync(fs, cancellationToken);
        }

        return relativePath;
    }

    public Task<(Stream Stream, string ContentType, string FileName)?> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_env.ContentRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath))
            return Task.FromResult<(Stream, string, string)?>(null);

        var ext = Path.GetExtension(fullPath);
        var contentType = ContentTypesByExtension.TryGetValue(ext, out var ct) ? ct : "application/octet-stream";
        var name = Path.GetFileName(fullPath);
        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        return Task.FromResult<(Stream, string, string)?>((stream, contentType, name));
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_env.ContentRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
