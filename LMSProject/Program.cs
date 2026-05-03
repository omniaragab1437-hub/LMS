using Microsoft.EntityFrameworkCore;
using MLSCore;
using MLSEF;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MLSCore.IdentityModel;
using LMSProject.Areas.Admin.Helpers;
using MLSCore.Interfaces;
using MLSEF.Repositories;
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

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());


var app = builder.Build();

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
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}")
    .WithStaticAssets();



app.Run();
