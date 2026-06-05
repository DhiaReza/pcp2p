using System.ComponentModel.DataAnnotations;

namespace pcp2p.Models
{
    public class TestPreset
    {
        [Key]
        public int Id {get;set;}
        // 1080p Medium, 1080p Ultra, 1440p Ultra, 4K Ultra
        public string Name {get;set;}
    }
}