using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Marketing;

public class CreateCouponDto
{
    [Required]
    [StringLength(64)]
    public string Code { get; set; } = string.Empty;

    public CouponType Type { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Value { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MinOrderAmount { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public int? TotalUsageLimit { get; set; }
    public int? PerUserUsageLimit { get; set; }

    public bool IsActive { get; set; } = true;
}
