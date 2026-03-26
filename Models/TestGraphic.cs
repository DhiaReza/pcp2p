using System.ComponentModel.DataAnnotations;

namespace pcp2p
{
    public class TestGraphic
    {
        [Key]
        public int Id {get;set;}
        // set to medium, low, high, ultra, etc
        public string Name {get;set;}
    }
}