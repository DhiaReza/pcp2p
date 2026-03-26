using CsvHelper.Configuration;

namespace pcp2p.Models
{
    public class GPU_Benchmark
    {
        public string Name {get;set;}
        public float Medium1080p {get;set;}
        public float Ultra1080p {get;set;}
        public float Ultra1440p {get;set;}
        public float Ultra4K {get;set;}
    }
    public sealed class GPU_Benchmark_Map : CsvClassMap<GPU_Benchmark>
    {
        public GPU_Benchmark_Map()
        {
            Map(m => m.Name).Name("GPU Name");
            Map(m => m.Medium1080p).Name("1080p Medium");
            Map(m => m.Ultra1080p).Name("1080p Ultra");
            Map(m => m.Ultra1440p).Name("1440p Ultra");
            Map(m => m.Ultra4K).Name("4K Ultra");
        }
        
    }
}