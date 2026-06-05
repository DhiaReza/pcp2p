using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class SelectHardwareDTO
    {
        public List<HardwareOptions>? hardwares;
        public List<TestSubject>? testSubjects;
        public List<TestPreset>? testPresets;
    }    
}