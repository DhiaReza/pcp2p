namespace pcp2p.Models
{
    public class AdminPageDTO
    {
        public List<Hardware> Cpus { get; set; } = new List<Hardware>();
        public List<Hardware> Gpus { get; set; } = new List<Hardware>();
        public List<Brand> Brands {get;set;} = new List<Brand>();
    }
}