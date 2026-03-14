var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// HttpClient nommés
builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:AuthService"]!);
});

builder.Services.AddHttpClient("ProfileService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:ProfileService"]!);
});

// Reverse proxy YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapControllers();
app.MapReverseProxy();

app.Run();