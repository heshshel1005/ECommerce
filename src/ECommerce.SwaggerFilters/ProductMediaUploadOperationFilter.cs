using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ECommerce.Swagger;

/// <summary>
/// Documents the product media upload endpoint with a file input in Swagger UI (multipart/form-data including "file").
/// </summary>
public class ProductMediaUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor descriptor)
            return;

        if (descriptor.ControllerName != "ProductMedia" || descriptor.ActionName != "UploadAsync")
            return;

        operation.RequestBody = new OpenApiRequestBody
        {
            Description = "Product image or video. File: image (jpg, png, gif, webp) or video (mp4, webm). Max 50 MB.",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Required = new HashSet<string> { "productId", "file", "mediaType" },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["productId"] = new OpenApiSchema { Type = "string", Format = "uuid", Description = "Product ID" },
                            ["file"] = new OpenApiSchema { Type = "string", Format = "binary", Description = "Image or video file" },
                            ["mediaType"] = new OpenApiSchema { Type = "integer", Format = "int32", Description = "0 = Image, 1 = Video" },
                            ["sortOrder"] = new OpenApiSchema { Type = "integer", Format = "int32", Description = "Display order" },
                            ["isPrimary"] = new OpenApiSchema { Type = "boolean", Description = "Use as primary media" },
                            ["altText"] = new OpenApiSchema { Type = "string", Description = "Alt text for accessibility", Nullable = true }
                        }
                    }
                }
            }
        };

        operation.Parameters?.Clear();
    }
}
