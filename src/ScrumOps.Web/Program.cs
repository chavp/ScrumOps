using ScrumOps.Web.Components;
using ScrumOps.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HTTP client services
var baseAddress = new Uri("http://localhost:8080");
builder.Services.AddHttpClient<ITeamService, TeamService>(client =>
{
    client.BaseAddress = baseAddress;
});

builder.Services.AddHttpClient<IProductBacklogService, ProductBacklogService>(client =>
{
    client.BaseAddress = baseAddress;
});

builder.Services.AddHttpClient<ISprintService, SprintService>(client =>
{
    client.BaseAddress = baseAddress;
});

// Add application services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
