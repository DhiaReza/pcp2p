using System.ComponentModel.DataAnnotations;

namespace pcp2p.Models
{
    public class TestSubject
    {
        [Key]
        public int Id {get;set;}
        // for GPU, set to raster, raytracing
        // for CPU set to gaming, single thread, and multi thread
        public string Name {get;set;}
    }
}