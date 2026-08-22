using ECommerce.Samples;
using Xunit;

namespace ECommerce.EntityFrameworkCore.Applications;

[Collection(ECommerceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<ECommerceEntityFrameworkCoreTestModule>
{

}
