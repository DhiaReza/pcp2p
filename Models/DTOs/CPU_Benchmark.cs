using System.Runtime.CompilerServices;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace pcp2p.Models
{
    public class CPU_Benchmark
    {
        public string Name {get;set;}
        public int FPS {get;set;}        
    }

    public sealed class CPU_Benchmark_Map : CsvClassMap<CPU_Benchmark>
    {
        public CPU_Benchmark_Map()
        {
            // We ignore "1024" and "FPS" by simply not mapping them
            Map(m => m.Name).Name("CPU Name");
            Map(m => m.FPS).Name("FPS");
        }
    }

}
