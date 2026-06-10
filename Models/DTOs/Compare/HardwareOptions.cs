using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class HardwareOptions
    {
        public int Id {get;set;}
        public string Name {get;set;} 
        public int? Vram {get;set;}
        public decimal? MSRP {get;set;}
        public int? CoreCount {get;set;}
        public int? ThreadCount {get;set;}
        public string? Socket {get;set;}
        public string? Generation {get;set;}
        public DateOnly ReleaseDate {get;set;}
        
    }
}