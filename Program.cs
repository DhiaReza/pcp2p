using pcp2p.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using pcp2p;
using Microsoft.AspNetCore.Identity;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore.Storage;

var builder = WebApplication.CreateBuilder(args);
string gpufilepath = "docs/Data/gpu_data.json";
string cpufilepath = "docs/Data/CPU_Cleaned_Data.csv";
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);

    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            await SeedData.SeedBrandAndType(context);
            await SeedData.SeedSource(context);
            await SeedData.SeedResolution(context);
            await SeedData.SeedGraphic(context);
            await SeedData.SeedTestSubject(context);
            await SeedData.SeedGPU(context, gpufilepath);
            await SeedData.SeedCPU(context, cpufilepath);
            await SeedData.SeedRolesAndAdmin(services);
            await SeedData.SeedCPUBenchmark2022(context, "docs/Data/CPU_2022_Benchmark@1080p.csv");
            await SeedData.SeedCPUBenchmark2025(context, "docs/Data/CPU_2025_Benchmark@1080p.csv");
            await SeedData.SeedCPUBenchmarInterpolated(context, "docs/Data/CPU_Interpolated_Benchmark@1080p.csv");
            await SeedData.SeedGPUBenchmark2022(context, "docs/Data/GPU_2022_Benchmark_Overall_Raster.csv");
            await SeedData.SeedGPUBenchmark2025(context, "docs/Data/GPU_2025_Benchmark_Overall_Raster.csv");
            await SeedData.SeedGPUBenchmarkInterpolated(context, "docs/Data/GPU_Interpolated_Benchmark_Overall_Raster.csv");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}

app.Run();