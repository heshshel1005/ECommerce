using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Payment;

/// <summary>
/// Payment app service: create intent, confirm payment, list gateways. All endpoints require auth.
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
public interface IPaymentAppService : IApplicationService
{
    /// <summary>List available payment gateways (e.g. Stripe, PayPal).</summary>
    Task<List<PaymentGatewayDto>> GetGatewaysAsync();

    /// <summary>Create a payment intent for an order. Returns client secret / gateway order id for frontend.</summary>
    Task<CreatePaymentIntentResult> CreatePaymentIntentAsync(Guid orderId, string gatewayName);

    /// <summary>Confirm/capture payment after client completed gateway flow.</summary>
    Task<ConfirmPaymentResult> ConfirmPaymentAsync(Guid orderId, string gatewayPaymentId);

    /// <summary>Refund an order (admin).</summary>
    Task<RefundPaymentResult> RefundAsync(Guid orderId, decimal? amount = null, string? reason = null);
}
