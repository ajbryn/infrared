namespace infrared.Contracts.Response;

public class SubscriptionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Timezone { get; set; } = TimeZoneInfo.Utc.Id;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
