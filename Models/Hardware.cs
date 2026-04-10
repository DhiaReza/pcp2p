using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pomelo.EntityFrameworkCore.MySql;

namespace pcp2p.Models
{
    public class Hardware
    {
        [Key]
        public int Id {get; set;}
        [Required]
        public string Name  {get; set;}

        [Column(TypeName = "decimal(18,2)")]
        public decimal MSRP { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter), "dd-MM-yyyy")]
        public DateOnly ReleaseDate {get;set;}
        public int TDP {get;set;}

        public Cpu Cpu {get;set;}
        public Gpu Gpu {get;set;}

        // link to hardware type
        public int HardwareTypeId {get;set;}
        public HardwareType HardwareType {get;set;}

        // link to brand

        public int BrandId {get;set;}
        public Brand Brand {get;set;}

        // link to Benchmark
        public ICollection<Benchmark> Benchmarks { get; set; } = new List<Benchmark>();

    }
}