namespace ECommerce.Marketing;

public class SendNewsletterCampaignDto
{
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsBodyHtml { get; set; } = true;
}
