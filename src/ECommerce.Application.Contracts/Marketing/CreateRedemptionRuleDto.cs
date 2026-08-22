namespace ECommerce.Marketing;

public class CreateRedemptionRuleDto
{
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; }
    public int PointsRequired { get; set; }
    public decimal Value { get; set; }
    public decimal MinOrderAmount { get; set; }
}
