using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class HardwareComparisonDTO
    {
        public Hardware Hw {get;set;}
        public Cpu? Cpu {get;set;}
        public Gpu? Gpu {get;set;}
        public double Score {get;set;}
        public double P2P {get;set;}
        public double P2PPercent {get;set;}
        public int? BenchDate {get;set;}
    }    
}