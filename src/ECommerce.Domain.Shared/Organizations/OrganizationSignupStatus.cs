namespace ECommerce.Organizations;

/// <summary>
/// Host-side workflow state for a public organization signup request.
/// </summary>
public enum OrganizationSignupStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
