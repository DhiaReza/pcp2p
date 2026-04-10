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
        // 1 for original, 2 for interpolated
        public int TestSourceId {get;set;}
        public TestSource TestSource {get;set;}
        public int HardwareId { get; set; }
        public Hardware Hardware { get; set; }
        // set to gaming, raster, RT, single thread, multi thread etc
        public TestSubject TestSubject {get;set;}
        public int TestSubjectId { get; set; }
        // just the score in number
        public float Score { get; set; }
        // set to 1080p, 1440p, or 4K
        public TestResolution TestResolution {get;set;}
        public int TestResolutionId { get; set; }
        // graphics settings,  low med high ulktra
        public TestGraphic? TestGraphic {get;set;}
        public int? TestGraphicId { get; set; }
    }
}
