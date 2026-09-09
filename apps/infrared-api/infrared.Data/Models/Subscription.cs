using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
