using ECommerce.Catalog;
using Xunit;

namespace ECommerce.EntityFrameworkCore.Applications;

[Collection(ECommerceTestConsts.CollectionDefinitionName)]
public class EfCoreBrandAppServiceTranslationCrudTests : BrandAppServiceTranslationCrudTests<ECommerceEntityFrameworkCoreTestModule>
{
}
