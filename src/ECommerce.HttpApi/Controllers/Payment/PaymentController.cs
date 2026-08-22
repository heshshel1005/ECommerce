using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Payment;

[Route("api/app/payment")]
[Area("app")]
[Authorize]
public class PaymentController : ECommerceController
{
    private readonly IPaymentAppService _paymentAppService;

    public PaymentController(IPaymentAppService paymentAppService)
    {
        _paymentAppService = paymentAppService;
    }

    /// <summary>List available payment gateways (e.g. Stripe, PayPal).</summary>
    [HttpGet("gateways")]
    public async Task<List<PaymentGatewayDto>> GetGatewaysAsync()
    {
        return await _paymentAppService.GetGatewaysAsync();
    }

    /// <summary>Create a payment intent for an order. Returns client secret / gateway order id for frontend. No card data is sent or stored.</summary>
    [HttpPost("create-intent")]
    public async Task<CreatePaymentIntentResult> CreatePaymentIntentAsync([FromBody] CreatePaymentIntentRequestDto input)
    {
        return await _paymentAppService.CreatePaymentIntentAsync(input.OrderId, input.GatewayName);
    }

    /// <summary>Confirm/capture payment after the client completed the gateway flow.</summary>
    [HttpPost("confirm")]
    public async Task<ConfirmPaymentResult> ConfirmPaymentAsync([FromBody] ConfirmPaymentRequestDto input)
    {
        return await _paymentAppService.ConfirmPaymentAsync(input.OrderId, input.GatewayPaymentId);
    }

    /// <summary>Refund an order (admin only).</summary>
    [HttpPost("refund")]
    [Authorize("ECommerce.Administration")]
    public async Task<RefundPaymentResult> RefundAsync([FromBody] RefundPaymentRequestDto input)
    {
        return await _paymentAppService.RefundAsync(input.OrderId, input.Amount, input.Reason);
    }
}

/// <summary>Request DTO for create payment intent.</summary>
public class CreatePaymentIntentRequestDto
{
    public Guid OrderId { get; set; }
    public string GatewayName { get; set; } = string.Empty;
}

/// <summary>Request DTO for confirm payment.</summary>
public class ConfirmPaymentRequestDto
{
    public Guid OrderId { get; set; }
    public string GatewayPaymentId { get; set; } = string.Empty;
}

/// <summary>Request DTO for refund.</summary>
public class RefundPaymentRequestDto
{
    public Guid OrderId { get; set; }
    public decimal? Amount { get; set; }
    public string? Reason { get; set; }
}
