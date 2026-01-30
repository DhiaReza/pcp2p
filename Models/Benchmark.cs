using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class Benchmark
    {

        // one to many with benchmark
        [Key]
        public int Id {get;set;}
        public string NameUppercase {get;set;}
        public string NameLowercase {get;set;}

        public string NameCapitalized {get;set;}
        public int Score {get;set;}
        public string Description {get;set;}
        public int HardwareId {get;set;}
        public Hardware Hardware {get;set;}
    }
}