using System;
using Volo.Abp.Application.Dtos;

namespace ECommerce.Customers;

public class CustomerProfileDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    /// <summary>Read-only; from Identity user.</summary>
    public string? Email { get; set; }
}
