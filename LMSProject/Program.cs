using Microsoft.EntityFrameworkCore;
using MLSCore;
using MLSEF;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MLSCore.IdentityModel;
using LMSProject.Areas.Admin.Helpers;
using MLSCore.Interfaces;
using MLSEF.Repositories;
//using LMSProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("conString"))
);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Options =>
{
    Options.Password.RequiredLength = 8;
    Options.Password.RequireNonAlphanumeric = true;
    Options.Password.RequireUppercase = true;
    Options.User.RequireUniqueEmail = true;
}
    ).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IInstructorRepository, InstructorRepository>();
//builder.Services.AddScoped<IRoleInitializationService, RoleInitializationService>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(7); 
});
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());


var app = builder.Build();

// Initialize roles and default SuperAdmin
using (var scope = app.Services.CreateScope())
{
    //var roleInitializationService = scope.ServiceProvider.GetRequiredService<IRoleInitializationService>();
    //await roleInitializationService.InitializeRolesAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
