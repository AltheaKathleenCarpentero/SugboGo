using System.ComponentModel.DataAnnotations;

namespace SugboGo.Models;

public sealed class TravelSpot
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(160)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(160)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [StringLength(600)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Region { get; set; } = string.Empty;

    public bool IsPopular { get; set; }
}
