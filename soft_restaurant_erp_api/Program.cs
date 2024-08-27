using ADO;
using soft_restaurant_erp_api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
