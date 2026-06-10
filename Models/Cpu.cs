using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class Cpu
    {
        [Key] // uses the same Id as hardware with 1:1 relationship
        public int HardwareId {get;set;}
        public Hardware Hardware {get;set;}
        public string CodeName {get;set;}
        public string Generation {get;set;}
        public string Socket {get;set;}
        public int CoreCount {get;set;}
        public int ThreadCount {get;set;}
        public float BaseClock {get;set;}
        public float TurboClock {get;set;}
        public float L1_Cache {get;set;}
        public float L2_Cache {get;set;}
        public float L3_Cache {get;set;}
    }
}