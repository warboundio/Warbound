using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;
using Data.Serialization;

namespace Data.BlizzardAPI.Endpoints;

[Table("journal_encounter", Schema = "wow")]
public sealed class JournalEncounter
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; } = -1;

    [MaxLength(255)]
    [EncodedField]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2047)] public string Items { get; set; } = string.Empty;

    [MaxLength(255)]
    [EncodedField]
    public string InstanceName { get; set; } = string.Empty;

    public int InstanceId { get; set; } = -1;
    [MaxLength(255)] public string CategoryType { get; set; } = string.Empty;
    [MaxLength(2047)] public string ModesTypes { get; set; } = string.Empty;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
