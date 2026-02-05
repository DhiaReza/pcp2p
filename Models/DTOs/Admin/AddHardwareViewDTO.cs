using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using pcp2p.Models;

namespace pcp2p.Models
{
    // Data sent to addhardware view
    public class AddHardwareViewDTO
    {
        // This holds the data the admin fills out
    public AddHardwareDTO HardwareData { get; set; }

    // These hold the data for the dropdowns
    public List<Brand> BrandOptions { get; set; } = new List<Brand>();
    public List<HardwareType> TypeOptions { get; set; } = new List<HardwareType>();
    }
}