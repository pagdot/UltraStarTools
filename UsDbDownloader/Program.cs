using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Shared;
using UsDbDownloader.Data;
using UsDbDownloader.Services;

var builder = WebApplication.CreateBuilder(args);

var songsFile = builder.Configuration.GetValue<string>("SongListFile");
var songs = JsonSerializer.Deserialize<List<CompleteSong>>(File.ReadAllText(songsFile)).ToList();

if (songs is null)
    throw new Exception($"SongListFile '[{songsFile}' has to exist!");

var dbPath = builder.Configuration.GetValue<string>("DbPath");
if (string.IsNullOrEmpty(dbPath))
    dbPath = "./UltraTools.db";

builder.Services.Configure<UsDbLoginModel>(builder.Configuration.GetSection("Login"));
builder.Services.Configure<SettingsModel>(builder.Configuration.GetSection("Settings"));

// Add services to the container.
builder.Services.AddDbContextFactory<UltraToolsContext>(o => o.UseSqlite( $"Data Source={dbPath}"));
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<List<CompleteSong>>(songs);
builder.Services.AddScoped<Repository>();
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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UltraToolsContext>();
    db.Database.Migrate();
}

app.Run();