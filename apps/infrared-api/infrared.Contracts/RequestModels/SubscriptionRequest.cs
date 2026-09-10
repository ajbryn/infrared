namespace infrared.Contracts.Request;

public class SubscriptionRequest
{
    public string Name { get; set; } = string.Empty;
    public string Timezone { get; set; } = TimeZoneInfo.Utc.Id;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
