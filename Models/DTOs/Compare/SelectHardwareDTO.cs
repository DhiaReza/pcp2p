using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pcp2p.Models;

namespace pcp2p.Models
{
    public class SelectHardwareDTO
    {
        [Required(ErrorMessage = "Please select at least one hardware")]
        public List<HardwareOptions>? hardwares;
        [Required(ErrorMessage = "Please select a category ")]
        public List<TestSubject>? testSubjects;
        [Required(ErrorMessage = "Please select a preset")]
        public List<TestPreset>? testPresets;
        public int hwtypeid {get;set;}
    }    
}