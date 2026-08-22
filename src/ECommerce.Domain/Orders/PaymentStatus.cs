namespace ECommerce.Orders;

/// <summary>
/// Payment state for an order. No raw card data is stored; gateway holds payment method.
/// </summary>
public enum PaymentStatus
{
    None = 0,
    Pending = 1,
    Authorized = 2,
    Paid = 3,
    Failed = 4,
    Refunded = 5,
    /// <summary>Customer will pay when the order is delivered.</summary>
    CashOnDelivery = 6,
}
