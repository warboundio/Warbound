using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Models;

[Table("journal_expansion", Schema = "wow")]
public sealed class JournalExpansion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(127)] public string Name { get; set; } = string.Empty;
    [MaxLength(2047)] public string DungeonIds { get; set; } = string.Empty;
    [MaxLength(2047)] public string RaidIds { get; set; } = string.Empty;
    public int ExpansionIdLUA { get; set; } = -1;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
