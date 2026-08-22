using ECommerce.Samples;
using Xunit;

namespace ECommerce.EntityFrameworkCore.Domains;

[Collection(ECommerceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<ECommerceEntityFrameworkCoreTestModule>
{

}
