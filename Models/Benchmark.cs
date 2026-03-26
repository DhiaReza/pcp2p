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
        public int? Date { get; set; }
        public int TestSourceId {get;set;}
        public TestSource TestSource {get;set;}
        public int HardwareId { get; set; }
        
        public Hardware Hardware { get; set; }
        public TestSubject TestSubject {get;set;}
        public int TestSubjectId { get; set; }
        public float Score { get; set; }
        public TestResolution TestResolution {get;set;}
        public int TestResolutionId { get; set; }
        public TestGraphic? TestGraphic {get;set;}
        public int? TestGraphicId { get; set; }
    }
}
