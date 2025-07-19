using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ETL.BlizzardAPI.Enums;

namespace ETL.BlizzardAPI.Endpoints;

[Table("profession_media", Schema = "wow")]
public sealed class ProfessionMedia
{
    [Key]
    public int Id { get; set; } = -1;
    [MaxLength(1023)] public string URL { get; set; } = string.Empty;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}