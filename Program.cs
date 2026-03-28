using Azure.Identity;
using KeyVaultDemo.Configurations;

var builder = WebApplication.CreateBuilder(args);

bool useAzureKeyVault = builder.Configuration.GetValue<bool>("FeatureFlags:UseAzureKeyVault");
bool useMongoDb = builder.Configuration.GetValue<bool>("FeatureFlags:UseMongoDb");

if (useAzureKeyVault)
{
    var keyVaultOptions = builder.Configuration
        .GetSection(AzureKeyVaultOptions.SectionName)
        .Get<AzureKeyVaultOptions>();

    var keyVaultUri = keyVaultOptions?.KeyVaultUri;

    if (string.IsNullOrEmpty(keyVaultUri))
        throw new InvalidOperationException("Key Vault URI is not configured.");

    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        new DefaultAzureCredential());

    Console.WriteLine("Using Azure Key Vault for configuration");
}

if (useMongoDb)
{
    Console.WriteLine("Using MongoDB repository");
}
else
{
    Console.WriteLine("Using in-memory repository");
}

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();