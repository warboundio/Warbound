using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;
using Data.Serialization;

namespace Data.BlizzardAPI.Endpoints;

[Table("quest", Schema = "wow")]
public sealed class Quest
{
    [Key]
    [EncodedField(3)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [MaxLength(511)]
    [EncodedField]
    public string Name { get; set; } = string.Empty;

    public QuestIdentifier QuestIdentifier { get; set; }
    public int QuestIdentifierId { get; set; }
    public int QuestTypeId { get; set; }
    [MaxLength(2047)] public string RewardItems { get; set; } = string.Empty;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
