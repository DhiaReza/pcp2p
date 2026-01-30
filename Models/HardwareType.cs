using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class HardwareType
    {
        // one to one woth hardware
        [Key]
        public int Id {get;set;}
        public string Name {get;set;}
        public string NameUppercase {get;set;}
        public string NameLowercase {get;set;}

        public string NameCapitalized {get;set;}

        // hardware key
        public ICollection<Hardware> Hardwares { get; set; } = new List<Hardware>();
    }
}