using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

[Table("realm", Schema = "wow")]
public sealed class Realm
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(63)] public string Name { get; set; } = string.Empty;
    [MaxLength(63)] public string Slug { get; set; } = string.Empty;
    [MaxLength(63)] public string Category { get; set; } = string.Empty;
    [MaxLength(63)] public string Locale { get; set; } = string.Empty;
    [MaxLength(63)] public string Timezone { get; set; } = string.Empty;
    [MaxLength(63)] public string Type { get; set; } = string.Empty;
    public bool IsTournament { get; set; }
    [MaxLength(63)] public string Region { get; set; } = string.Empty;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
