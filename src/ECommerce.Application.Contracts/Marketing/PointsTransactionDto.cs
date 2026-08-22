using System;
using Volo.Abp.Application.Dtos;

namespace ECommerce.Marketing;

public class PointsTransactionDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }
    public int Amount { get; set; }
    public int Type { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? RedemptionRuleId { get; set; }
    public string? Description { get; set; }
    public DateTime CreationTime { get; set; }
}
