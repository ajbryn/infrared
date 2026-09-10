using System.ComponentModel.DataAnnotations;

namespace infrared.Data.Models;

public class Subscription
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = string.Empty;
    [MaxLength(50)]
    [Required]
    public string Timezone { get; set; } = TimeZoneInfo.Utc.Id;
    [Required]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
