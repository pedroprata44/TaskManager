using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Dtos;

namespace TaskManager.Tests.Dtos;

public class TaskDtoTests
{
    [Fact]
    public void TaskDto_ValidData_PassesValidation()
    {
        var dto = new TaskDto
        {
            UserId = Guid.NewGuid(),
            Title = CreateRandomValue("Title"),
            Description = CreateRandomValue("Description"),
            IsCompleted = true
        };

        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, validateAllProperties: true);

        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TaskDto_InvalidData_FailsValidation()
    {
        var dto = new TaskDto
        {
            UserId = null,
            Title = string.Empty,
            Description = string.Empty
        };

        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, validateAllProperties: true);

        Assert.False(isValid);
        Assert.Contains(validationResults, result => result.MemberNames.Contains("Title"));
        Assert.Contains(validationResults, result => result.MemberNames.Contains("Description"));
        Assert.Contains(validationResults, result => result.MemberNames.Contains("UserId"));
    }

    private static string CreateRandomValue(string prefix)
    {
        return $"{prefix}-{Guid.NewGuid():N}";
    }
}
