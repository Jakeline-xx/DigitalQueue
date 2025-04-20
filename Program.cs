using DigitalQueue.Domain.Interfaces;
using DigitalQueue.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IQueueService, QueueService>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.Run();