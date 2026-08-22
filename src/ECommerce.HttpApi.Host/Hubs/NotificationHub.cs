using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.MultiTenancy;

namespace ECommerce.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ICurrentTenant _currentTenant;

    public NotificationHub(ICurrentTenant currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public override async Task OnConnectedAsync()
    {
        var groupName = GetTenantGroupName();
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var groupName = GetTenantGroupName();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await base.OnDisconnectedAsync(exception);
    }

    private string GetTenantGroupName() =>
        _currentTenant.Id.HasValue
            ? $"tenant-{_currentTenant.Id.Value}"
            : "tenant-host";
}
