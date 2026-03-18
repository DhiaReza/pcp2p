using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class Gpu
    {
        [Key] // uses the same id as Hardware 
        public int HardwareId {get;set;}
        public Hardware Hardware {get;set;}
        public string Generation {get;set;}
        public string Architecture {get;set;}
        public int Vram {get;set;}
        public int? BaseClock {get;set;}
        public int? BoostClock {get;set;}
        public int? GameClock {get;set;}
    }
}