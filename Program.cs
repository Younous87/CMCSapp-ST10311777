using CMCSapp_ST10311777.Models;
using CMCSapp_ST10311777.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<BlobService>();
// Register DatabaseService with the connection string
builder.Services.AddSingleton<ClaimTable>();
builder.Services.AddSingleton<LecturerTable>();
builder.Services.AddSingleton<DocumentTable>();
builder.Services.AddScoped<ClaimVerificationService>();
builder.Services.AddScoped<ClaimProcessingService>();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureStorage:ConnectionString:blob"]!, preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["AzureStorage:ConnectionString:queue"]!, preferMsi: true);
});
var app = builder.Build();



var connectionString = app.Configuration.GetConnectionString("AzureSQLDatabase")!;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
