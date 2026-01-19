using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class Cpu
    {
        [Key] // uses the same Id as hardware with 1:1 relationship
        public int HardwareId {get;set;}
        public Hardware Hardware {get;set;}
        public int CoreCount {get;set;}
        public int ThreadCount {get;set;}
        public int BaseClock {get;set;}
        public int BoostClock {get;set;}
        public int TDP {get;set;}
    }
}