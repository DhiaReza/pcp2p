using System.ComponentModel.DataAnnotations;

namespace pcp2p.Models
{
    public class HardwareTypeDTO
    {
        [Required(ErrorMessage ="Please fill the Hardware type")]
        public string Name {get;set;}
    }
}