using System;
using Volo.Abp.Application.Dtos;

namespace ECommerce.Marketing;

public class CustomerPointsDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }
    public int Balance { get; set; }
    public string? Tier { get; set; }
}
