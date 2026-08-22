using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.OrganizationSignup;

/// <summary>
/// Stores and retrieves organization signup logo files on the backend. Implemented in the host (e.g. local disk under App_Data).
/// </summary>
public interface IOrganizationSignupLogoStorage
{
    /// <summary>
    /// Save a file and return the relative path to store in the database (e.g. "App_Data/OrganizationSignupLogos/{requestId}/{fileName}").
    /// </summary>
    Task<string> SaveAsync(Stream stream, string fileName, Guid requestId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Open a file for reading by its stored relative path. Returns stream, content type, and suggested file name.
    /// </summary>
    Task<(Stream Stream, string ContentType, string FileName)?> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete the file at the given relative path if it exists.
    /// </summary>
    Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default);
}
