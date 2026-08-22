using Xunit;

namespace ECommerce.EntityFrameworkCore;

[CollectionDefinition(ECommerceTestConsts.CollectionDefinitionName)]
public class ECommerceEntityFrameworkCoreCollection : ICollectionFixture<ECommerceEntityFrameworkCoreFixture>
{

}
