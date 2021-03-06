using System.Security.Claims;
using BasicWorkManager.Models;
using BasicWorkManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<DataBaseManager>();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddAuthentication("SID").AddCookie("SID", options =>
{
    options.Cookie.Name = "SID";
	options.Cookie.IsEssential = true;
	options.AccessDeniedPath = "/Index";
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("Admin",
		policy => policy.RequireClaim(ClaimTypes.Role, ((int)UserRole.Boss).ToString()));

	options.AddPolicy("Moderator",
		policy => policy.RequireClaim(ClaimTypes.Role,
			new[] { ((int)UserRole.Supervisor).ToString(), ((int)UserRole.Boss).ToString() }));

	options.AddPolicy("Employee",
		policy => policy.RequireClaim("Company"));
});

builder.Services.AddHsts(options =>
{
	//options.Preload = true;
	//options.IncludeSubDomains = true;
	options.MaxAge = TimeSpan.FromHours(3);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // todo - The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();