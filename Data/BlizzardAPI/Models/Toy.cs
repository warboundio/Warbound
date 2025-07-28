using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;
using Data.Serialization;

namespace Data.BlizzardAPI.Models;

[Table("toy", Schema = "wow")]
public sealed class Toy
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [EncodedField(3)]
    public int Id { get; set; }

    [EncodedField]
    [MaxLength(127)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(31)]
    public string SourceType { get; set; } = string.Empty;

    public int MediaId { get; set; } = -1;

    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
