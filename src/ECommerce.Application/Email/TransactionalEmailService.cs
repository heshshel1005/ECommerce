using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Emailing;

namespace ECommerce.Email;

public class TransactionalEmailService : ITransactionalEmailService
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<TransactionalEmailService> _logger;

    public TransactionalEmailService(IEmailSender emailSender, ILogger<TransactionalEmailService> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(Guid orderId, string toEmail, string contactName, decimal total, IReadOnlyList<OrderLineInfo> lines)
    {
        var subject = "Order Confirmation";
        var body = BuildOrderConfirmationBody(orderId, contactName, total, lines);
        try
        {
            await _emailSender.SendAsync(toEmail, subject, body, isBodyHtml: true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send order confirmation email to {Email} for order {OrderId}", toEmail, orderId);
        }
    }

    public async Task SendShippingNotificationAsync(Guid orderId, string toEmail, string contactName, string? trackingInfo = null)
    {
        var subject = "Your order has shipped";
        var body = BuildShippingNotificationBody(orderId, contactName, trackingInfo);
        try
        {
            await _emailSender.SendAsync(toEmail, subject, body, isBodyHtml: true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send shipping notification to {Email} for order {OrderId}", toEmail, orderId);
        }
    }

    private static string BuildOrderConfirmationBody(Guid orderId, string contactName, decimal total, IReadOnlyList<OrderLineInfo> lines)
    {
        var name = string.IsNullOrWhiteSpace(contactName) ? "Customer" : contactName;
        var rows = lines.Count == 0
            ? "<tr><td colspan=\"4\">No items</td></tr>"
            : string.Join("", lines.Select(l =>
                $"<tr><td>{System.Net.WebUtility.HtmlEncode(l.ProductName)}</td><td>{System.Net.WebUtility.HtmlEncode(l.Sku)}</td><td>{l.Quantity}</td><td>{l.LineTotal:N2}</td></tr>"));
        return $@"
<!DOCTYPE html>
<html>
<head><meta charset=""utf-8""/><title>Order Confirmation</title></head>
<body>
<h1>Order Confirmation</h1>
<p>Hello {System.Net.WebUtility.HtmlEncode(name)},</p>
<p>Thank you for your order. Your order ID is <strong>{orderId:N}</strong>.</p>
<table border=""1"" cellpadding=""4"" cellspacing=""0"">
<thead><tr><th>Product</th><th>SKU</th><th>Qty</th><th>Total</th></tr></thead>
<tbody>{rows}</tbody>
</table>
<p><strong>Order total: {total:N2}</strong></p>
<p>We will notify you when your order ships.</p>
</body>
</html>";
    }

    private static string BuildShippingNotificationBody(Guid orderId, string contactName, string? trackingInfo)
    {
        var name = string.IsNullOrWhiteSpace(contactName) ? "Customer" : contactName;
        var tracking = string.IsNullOrWhiteSpace(trackingInfo)
            ? ""
            : $"<p>Tracking: <strong>{System.Net.WebUtility.HtmlEncode(trackingInfo)}</strong></p>";
        return $@"
<!DOCTYPE html>
<html>
<head><meta charset=""utf-8""/><title>Order Shipped</title></head>
<body>
<h1>Your order has shipped</h1>
<p>Hello {System.Net.WebUtility.HtmlEncode(name)},</p>
<p>Your order <strong>{orderId:N}</strong> has been shipped.</p>
{tracking}
<p>Thank you for shopping with us.</p>
</body>
</html>";
    }
}
