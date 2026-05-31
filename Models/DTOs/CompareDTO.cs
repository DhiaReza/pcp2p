using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class CompareDTO
    {
        public string hardwareType;
        public List<Hardware> hardwares;
        public int testSubjectId;
        public List<TestSubject> testSibjects;
        public int TestResolutionId;
        public List<TestResolution> testResolutions;
        public int TestGraphicId;
        public List<TestGraphic> testGraphics;
        public int score;
    }
}