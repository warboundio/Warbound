using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

[Table("quest", Schema = "wow")]
public sealed class Quest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(511)] public string Name { get; set; } = string.Empty;
    public QuestIdentifier QuestIdentifier { get; set; }
    public int QuestIdentifierId { get; set; }
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
