using System.Runtime.CompilerServices;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace pcp2p.Models
{
    public class CPUSeedDTO
    {
        public string Name {get;set;}
        public string Brand {get;set;}
        public string Socket {get;set;}
        public string ReleaseDate {get;set;}
        public float BaseClock {get;set;}
        public float TurboClock {get;set;}
        public int TDP {get;set;}
        public string Codename {get;set;}
        public string Generation {get;set;}
        public int CoreCount {get;set;}
        public int ThreadCount {get;set;}
        public float L1Cache {get;set;}
        public float L2Cache {get;set;}
        public float L3Cache {get;set;}
        public decimal MSRP {get;set;}
        
    }

    public sealed class CPUMap : CsvClassMap<CPUSeedDTO>
    {
        public CPUMap()
        {
            // We ignore "1024" and "FPS" by simply not mapping them
            Map(m => m.Name).Name("Name");
            Map(m => m.Brand).Name("Brand");
            Map(m => m.Socket).Name("Socket");
            Map(m => m.ReleaseDate).Name("ReleaseDate");
            Map(m => m.BaseClock).Name("BaseClock");
            Map(m => m.TurboClock).Name("TurboClock");
            Map(m => m.TDP).Name("TDP");
            Map(m => m.Codename).Name("Codename");
            Map(m => m.Generation).Name("Generation");
            Map(m => m.CoreCount).Name("CoreCount");
            Map(m => m.ThreadCount).Name("ThreadCount");
            Map(m => m.L1Cache).Name("L1Cache");
            Map(m => m.L2Cache).Name("L2Cache");
            Map(m => m.L3Cache).Name("L3Cache");
            Map(m => m.MSRP).Name("MSRP");
        }
    }

}
