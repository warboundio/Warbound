using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

[Table("quest_category", Schema = "wow")]
public sealed class QuestCategory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(255)] public string Name { get; set; } = string.Empty;
    public ETLStateType Status { get; set; } = ETLStateType.COMPLETE;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
