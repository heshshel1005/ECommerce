using System;
using Volo.Abp.Application.Dtos;

namespace ECommerce.Marketing;

public class CouponDto : EntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public int Type { get; set; }
    public decimal Value { get; set; }
    public decimal MinOrderAmount { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public int? TotalUsageLimit { get; set; }
    public int? PerUserUsageLimit { get; set; }
    public bool IsActive { get; set; }
}
