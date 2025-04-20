using DigitalQueue.Domain.Interfaces;
using DigitalQueue.Application.Services;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IQueueService, QueueService>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error");

app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();