using UsDbDownloader.Data;
using UsDbDownloader.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<UsDbLoginModel>(builder.Configuration.GetSection("Login"));
builder.Services.Configure<SettingsModel>(builder.Configuration.GetSection("Settings"));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<UsDbService>();
builder.Services.AddHostedService<UsDownloaderService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();