using System;

namespace ECommerce.Account;

/// <summary>
/// Input for confirming a user's email with the token sent by email.
/// </summary>
public class ConfirmEmailInputDto
{
    public Guid UserId { get; set; }

    public string Token { get; set; } = string.Empty;
}
