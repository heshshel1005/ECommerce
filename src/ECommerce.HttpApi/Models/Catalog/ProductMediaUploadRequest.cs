using System;
using ECommerce.Catalog;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Models.Catalog;

/// <summary>
/// Request model for product media upload (multipart/form-data). Used with [FromForm].
/// </summary>
public class ProductMediaUploadRequest
{
    public Guid ProductId { get; set; }
    public IFormFile File { get; set; } = null!;
    public ProductMediaType MediaType { get; set; }
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
    public string? AltText { get; set; }
}
