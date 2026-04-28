using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Tests.Endpoints;

public class TaskEndpointTests : IClassFixture<TaskApiFactory>
{
    private readonly HttpClient _client;

    public TaskEndpointTests(TaskApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTask_GetById_ReturnsCreatedTask()
    {
        var task = CreateRandomTask("Create");

        var createResponse = await _client.PostAsJsonAsync("/task/create", task);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskModel>();
        Assert.NotNull(createdTask);
        Assert.Equal(task.Title, createdTask!.Title);
        Assert.Equal(task.Description, createdTask.Description);

        var getResponse = await _client.GetAsync($"/task/get/{createdTask.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetchedTask = await getResponse.Content.ReadFromJsonAsync<TaskModel>();
        Assert.NotNull(fetchedTask);
        Assert.Equal(createdTask.Id, fetchedTask!.Id);
        Assert.Equal(createdTask.Title, fetchedTask.Title);
    }

    [Fact]
    public async Task GetAllTasks_ReturnsCreatedTasks()
    {
        var firstTask = CreateRandomTask("First");
        var secondTask = CreateRandomTask("Second");

        await _client.PostAsJsonAsync("/task/create", firstTask);
        await _client.PostAsJsonAsync("/task/create", secondTask);

        var response = await _client.GetAsync("/task/getall");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskModel>>();
        Assert.NotNull(tasks);
        Assert.True(tasks!.Count >= 2);
        Assert.Contains(tasks, t => t.Title == firstTask.Title);
        Assert.Contains(tasks, t => t.Title == secondTask.Title);
    }

    [Fact]
    public async Task UpdateTask_ReturnsNoContentAndPersistsChanges()
    {
        var task = CreateRandomTask("Update");
        var createResponse = await _client.PostAsJsonAsync("/task/create", task);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskModel>();

        createdTask!.Description = CreateRandomValue("UpdatedDescription");
        var updateResponse = await _client.PutAsJsonAsync($"/task/update/{createdTask.Id}", createdTask);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/task/get/{createdTask.Id}");
        var updatedTask = await getResponse.Content.ReadFromJsonAsync<TaskModel>();

        Assert.NotNull(updatedTask);
        Assert.Equal(createdTask.Description, updatedTask!.Description);
    }

    [Fact]
    public async Task UpdateTask_WithMismatchedId_ReturnsBadRequest()
    {
        var task = CreateRandomTask("Mismatch");
        var createResponse = await _client.PostAsJsonAsync("/task/create", task);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskModel>();

        var badTask = new TaskModel
        {
            Id = Guid.NewGuid(),
            Title = createdTask!.Title,
            Description = createdTask.Description
        };

        var updateResponse = await _client.PutAsJsonAsync($"/task/update/{createdTask.Id}", badTask);
        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNoContent_ThenGetReturnsNotFound()
    {
        var task = CreateRandomTask("Delete");
        var createResponse = await _client.PostAsJsonAsync("/task/create", task);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskModel>();

        var deleteResponse = await _client.DeleteAsync($"/task/delete/{createdTask!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/task/get/{createdTask.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/task/get/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNonExistingTask_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/task/delete/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static TaskModel CreateRandomTask(string prefix)
    {
        return new TaskModel
        {
            Title = CreateRandomValue(prefix),
            Description = CreateRandomValue("Description")
        };
    }

    private static string CreateRandomValue(string prefix)
    {
        return $"{prefix}-{Guid.NewGuid():N}";
    }
}

public class TaskApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TaskDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TaskDbContext>(options =>
                options.UseInMemoryDatabase("TaskManagerEndpointTests"));

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
            dbContext.Database.EnsureCreated();
        });
    }
}
