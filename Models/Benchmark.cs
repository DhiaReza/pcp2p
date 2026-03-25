using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pcp2p.Models
{
    public class Benchmark
    {
        [Key]
        public int Id { get; set; }

        // e.g., "RTX 4090 Raster"
        public string Name { get; set; }
        // Set to year only , yyyy
        public int Date { get; set; }
        public int TestTypeId {get;set;}
        public TestSource TestSource {get;set;}
        public int HardwareId { get; set; }
        
        public Hardware Hardware { get; set; }

        // The specific test type (Raster, Ray Tracing, Gaming, etc.)
        public string TestSubject { get; set; }
        // could be fps or just plain score for synthetic benchmark
        public int Score { get; set; }


        // Resolution: e.g., 1440p, 4K, or 1080p
        public string Resolution { get; set; }

        // Graphics Setting: e.g., "Medium", "Ultra"
        // Note: You mentioned 1440p/4K only support Ultra. 
        // We store the actual setting used, not the rule.
        public string GraphicsSetting { get; set; }
    }
}
