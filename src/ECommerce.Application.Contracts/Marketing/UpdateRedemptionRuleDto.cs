namespace ECommerce.Marketing;

public class UpdateRedemptionRuleDto
{
    public string Name { get; set; } = string.Empty;
    public int PointsRequired { get; set; }
    public decimal Value { get; set; }
    public decimal MinOrderAmount { get; set; }
    public bool IsActive { get; set; }
}
