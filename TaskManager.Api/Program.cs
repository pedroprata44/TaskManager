using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Interfaces.Repositories;
using TaskManager.Api.Interfaces.Services;
using TaskManager.Api.Models;
using TaskManager.Api.Repositories;
using TaskManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/task/getall", (ITaskService taskService) => Results.Ok(taskService.GetAll()));

app.MapGet("/task/get/{id}", (Guid id, ITaskService taskService) =>
{
    try
    {
        return Results.Ok(taskService.GetById(id));
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
});

app.MapPost("/task/create", (TaskModel task, ITaskService taskService) =>
{
    taskService.Create(task);
    return Results.Created($"/task/get/{task.Id}", task);
});

app.MapPut("/task/update/{id}", (Guid id, TaskModel task, ITaskService taskService) =>
{
    if (id != task.Id)
    {
        return Results.BadRequest();
    }

    try
    {
        taskService.Update(task);
        return Results.NoContent();
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
});

app.MapDelete("/task/delete/{id}", (Guid id, ITaskService taskService) =>
{
    try
    {
        taskService.Delete(id);
        return Results.NoContent();
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

public partial class Program { }
