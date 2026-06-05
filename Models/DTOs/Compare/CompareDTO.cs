using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class CompareDTO
    {
        public List<Hardware>? hardwares;
        public List<TestSubject>? testSubjects;
        public List<TestResolution>? testResolutions;
        public List<TestGraphic>? testGraphics;
    }
}