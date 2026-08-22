using Volo.Abp.Settings;

namespace ECommerce.Settings;

public class ECommerceSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(new SettingDefinition(
            ECommerceSettings.Loyalty.PointsPerCurrencyUnit,
            "1"));
    }
}
