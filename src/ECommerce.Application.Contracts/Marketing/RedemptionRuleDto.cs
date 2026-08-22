using System;
using Volo.Abp.Application.Dtos;

namespace ECommerce.Marketing;

public class RedemptionRuleDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; }
    public int PointsRequired { get; set; }
    public decimal Value { get; set; }
    public decimal MinOrderAmount { get; set; }
    public bool IsActive { get; set; }
}
