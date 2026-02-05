using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using pcp2p.Models;

namespace pcp2p.Models
{
public class AddHardwareDTO
{
    // This DTO is used for adding hardware
    // Base hardware fields
    
    [Required(ErrorMessage = "Please fill hardware name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Please fill hardware generation")]
    public string Generation { get; set; }

    [Required(ErrorMessage = "Please fill hardware MSRP")]
    public decimal MSRP { get; set; }

    [Required(ErrorMessage = "Please fill hardware release date")]
    public DateTime ReleaseDate { get; set; }

    // Use IDs for the dropdown selections
    [Required(ErrorMessage = "Please select a hardware type")]
    public int HardwareTypeId { get; set; }

    [Required(ErrorMessage = "Please select a brand")]
    public int BrandId { get; set; }

    // GPU Specific (Nullable so they don't trigger validation for CPUs) ---
    public int? Vram { get; set; }
    public int? GpuBaseClock { get; set; }
    public int? BoostClock { get; set; }
    public int? GameClock { get; set; }
    
    // CPU Specific (Nullable) ---
    public int? CoreCount { get; set; }
    public int? ThreadCount { get; set; }
    public int? CpuBaseClock { get; set; }

    // Shared 
    public int? TDP { get; set; }
}
}