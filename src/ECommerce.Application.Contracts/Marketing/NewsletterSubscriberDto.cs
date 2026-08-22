using System;
using Volo.Abp.Application.Dtos;

namespace ECommerce.Marketing;

public class NewsletterSubscriberDto : EntityDto<Guid>
{
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? UnsubscribedAt { get; set; }
}
