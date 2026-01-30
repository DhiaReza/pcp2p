using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class BenchmarkDTO
    {
        [Required(ErrorMessage ="Please fill the benchmark name")]
        public string Name {get;set;}
        [Required(ErrorMessage ="Please fill the benchmark score")]
        public int Score {get;set;}
        public string Description {get;set;}
    }
}