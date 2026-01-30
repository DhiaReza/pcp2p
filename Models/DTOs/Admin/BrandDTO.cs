using System.ComponentModel.DataAnnotations;

namespace pcp2p.Models
{
    public class BrandDTO
    {
        [Required(ErrorMessage ="Please fill the Brand name")]
        public string Name {get;set;}
    }
}