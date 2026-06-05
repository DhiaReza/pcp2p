using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class HardwareComparison
    {
        public List<SelectedHardware>? hardwares;
        public List<TestSubject>? testSubjects;
        public List<TestPreset>? testPresets;
    }    
}