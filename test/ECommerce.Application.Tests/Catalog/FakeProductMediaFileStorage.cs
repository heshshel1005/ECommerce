using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Catalog;

/// <summary>
/// Test double: host registers <see cref="IProductMediaFileStorage"/>; application integration tests do not load the host module.
/// </summary>
public sealed class FakeProductMediaFileStorage : IProductMediaFileStorage
{
    public Task<string> SaveAsync(Stream stream, string fileName, Guid productId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"ProductMedia/{productId:N}/{fileName}");
    }

    public Task<(Stream Stream, string ContentType, string FileName)?> OpenReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<(Stream, string, string)?>(null);
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
