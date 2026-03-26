using System.ComponentModel.DataAnnotations;

namespace pcp2p.Models
{
    public class TestSubject
    {
        [Key]
        public int Id {get;set;}
        // set to gaming, raster, etc
        public string Name {get;set;}
    }
}