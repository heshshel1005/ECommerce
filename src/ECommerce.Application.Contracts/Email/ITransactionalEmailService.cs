using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Email;

/// <summary>
/// Sends transactional emails (order confirmation, shipping) via ABP IEmailSender.
/// </summary>
public interface ITransactionalEmailService
{
    Task SendOrderConfirmationAsync(Guid orderId, string toEmail, string contactName, decimal total, IReadOnlyList<OrderLineInfo> lines);
    Task SendShippingNotificationAsync(Guid orderId, string toEmail, string contactName, string? trackingInfo = null);
}

public class OrderLineInfo
{
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
