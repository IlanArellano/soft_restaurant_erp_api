using ADO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using soft_restaurant_erp_api.Middlewares;
using Microsoft.IdentityModel.Tokens;
using soft_restaurant_erp_api;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Authentication.TOKEN_SECRET))
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.SetDependencies(builder.Configuration);

builder.Services.AddSingleton<ResponseMiddleware>();
builder.Services.AddSingleton<RewindMiddleware>();

var app = builder.Build();

//Response Middlewares
app.UseMiddleware<RewindMiddleware>();
app.UseMiddleware<ResponseMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller=Home}/{action=Index}/{id?}");

app.Run();
