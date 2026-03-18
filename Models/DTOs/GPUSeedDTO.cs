namespace pcp2p.Models
{
    public class GPUSeedDTO
    {
        public string Name {get;set;}
        public string Generation {get;set;}
        public string Architecture {get;set;}
        public decimal MSRP {get;set;}
        public string ReleaseDate {get;set;}
        public string Brand {get;set;}
        public string HardwareType {get;set;}
        public GpuDto Gpu{get;set;}
        public class GpuDto
        {
            public int Vram { get; set; }
            public int? BaseClock { get; set; }
            public int? BoostClock { get; set; }
            public int? GameClock {get;set;}
            public int TDP { get; set; }
        }
    }
}