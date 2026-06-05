using System.Runtime.CompilerServices;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace pcp2p.Models
{
    public class CPU_Benchmark
    {
        public string Name {get;set;}
        public float FPS {get;set;}        
    }

    public sealed class CPU_Benchmark_Map : CsvClassMap<CPU_Benchmark>
    {
        public CPU_Benchmark_Map()
        {
            Map(m => m.Name).Name("CPU Name");
            Map(m => m.FPS).Name("FPS");
        }
    }

}
