using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel.DataAnnotations;
using ScrumOps.Web.Components.TeamManagement;

namespace ScrumOps.Web.Tests;

/// <summary>
/// Basic unit tests for Blazor Web application components and validation.
/// </summary>
public class WebComponentValidationTests
{
    [Fact]
    public void TeamFormModel_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var model = new TeamFormComponent.TeamFormModel
        {
            Name = "Valid Team Name",
            Description = "This is a valid team description",
            SprintLengthWeeks = 2
        };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TeamFormModel_WithEmptyName_ShouldFailValidation()
    {
        // Arrange
        var model = new TeamFormComponent.TeamFormModel
        {
            Name = "", // Invalid
            Description = "Valid description",
            SprintLengthWeeks = 2
        };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Name)));
    }

    [Theory]
    [InlineData(0)] // Too low
    [InlineData(5)] // Too high
    public void TeamFormModel_WithInvalidSprintLength_ShouldFailValidation(int sprintLength)
    {
        // Arrange
        var model = new TeamFormComponent.TeamFormModel
        {
            Name = "Valid Team",
            Description = "Valid description",
            SprintLengthWeeks = sprintLength
        };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.SprintLengthWeeks)));
    }

    [Fact]
    public void TeamFormModel_WithTooLongName_ShouldFailValidation()
    {
        // Arrange
        var model = new TeamFormComponent.TeamFormModel
        {
            Name = new string('A', 51), // Too long (max 50)
            Description = "Valid description",
            SprintLengthWeeks = 2
        };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Name)));
    }

    [Fact]
    public void TeamFormModel_WithTooLongDescription_ShouldFailValidation()
    {
        // Arrange
        var model = new TeamFormComponent.TeamFormModel
        {
            Name = "Valid Team",
            Description = new string('D', 501), // Too long (max 500)
            SprintLengthWeeks = 2
        };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Description)));
    }

    private static List<ValidationResult> ValidateModel<T>(T model)
    {
        var validationResults = new List<ValidationResult>();
        if (model != null)
        {
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
        }
        return validationResults;
    }
}