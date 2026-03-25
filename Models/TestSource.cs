using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pcp2p.Models
{
    public class TestSource
    {
        [Key]
        public int Id { get; set; }
        // original or interpolated
        public string Name {get;set;}
    }
}
