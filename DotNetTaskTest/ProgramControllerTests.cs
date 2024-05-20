using DotNetTask.Controllers;
using DotNetTask.DTO;
using DotNetTask.Models;
using DotNetTask.Models.DatabaseModels;
using DotNetTask.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

public class ProgramControllerTests
{
    private readonly Mock<ILogger<ProgramController>> _loggerMock;
    private readonly Mock<DotNetTaskDbContext> _dbContextMock;
    private readonly ProgramController _controller;

    public ProgramControllerTests()
    {
        _loggerMock = new Mock<ILogger<ProgramController>>();
        _dbContextMock = new Mock<DotNetTaskDbContext>();
        _controller = new ProgramController(_loggerMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public void SaveProgram_ShouldSaveProgramAndQuestions()
    {
        // Arrange
        var programTemplate = new ProgramTemplateDTO
        {
            ProgramTitle = "Sample Program",
            ProgramDescription = "Description",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        var questions = new List<QuestionDTO>
        {
            new QuestionDTO { Question = "What is your favorite color?", QuestionType = QuestionType.MultipleChoice, QuestionString = JsonSerializer.Serialize(new List<string> { "Red", "Blue" }) },
            new QuestionDTO { Question = "Describe your experience.", QuestionType = QuestionType.Text }
        };

        var programAndQuestionDTO = new ProgramAndQuestionDTO
        {
            ProgramTemplate = programTemplate,
            Questions = questions
        };

        // Act
        var result = _controller.SaveProgram(programAndQuestionDTO);

        // Assert
        Assert.IsType<OkResult>(result);
        _dbContextMock.Verify(x => x.Add(It.IsAny<ProgramTemplate>()), Times.Once);
        _dbContextMock.Verify(x => x.Add(It.IsAny<Questions>()), Times.Exactly(questions.Count));
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Exactly(questions.Count + 1));
    }

    [Fact]
    public void EditProgram_ShouldUpdateProgramAndQuestions()
    {
        // Arrange
        var programTemplateId = 1;
        var existingProgramTemplate = new ProgramTemplate
        {
            Id = programTemplateId,
            ProgramTitle = "Old Title",
            ProgramDescription = "Old Description",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        var programTemplate = new ProgramTemplateDTO
        {
            ProgramTitle = "New Title",
            ProgramDescription = "New Description",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        var questions = new List<QuestionDTO>
        {
            new QuestionDTO { Question = "What is your new favorite color?", QuestionType = QuestionType.MultipleChoice, QuestionString = JsonSerializer.Serialize(new List<string> { "Green", "Yellow" }) },
            new QuestionDTO { Question = "Describe your new experience.", QuestionType = QuestionType.Text }
        };

        var programAndQuestionDTO = new ProgramAndQuestionDTO
        {
            ProgramTemplate = programTemplate,
            Questions = questions
        };

        _dbContextMock.Setup(x => x.ProgramTemplate.Find(programTemplateId)).Returns(existingProgramTemplate);

        // Act
        var result = _controller.EditProgram(programTemplateId, programAndQuestionDTO);

        // Assert
        Assert.IsType<OkResult>(result);
        _dbContextMock.Verify(x => x.Update(existingProgramTemplate), Times.Once);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void DeleteProgram_ShouldRemoveProgramAndQuestions()
    {
        // Arrange
        var programTemplateId = 1;
        var existingProgramTemplate = new ProgramTemplate
        {
            Id = programTemplateId,
            ProgramTitle = "Sample Program",
            ProgramDescription = "Description",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        var questionsMappings = new List<QuestionsMapping>
        {
            new QuestionsMapping { Id = 1, ProgramTemplateId = programTemplateId, QuestionsId = 1 },
            new QuestionsMapping { Id = 2, ProgramTemplateId = programTemplateId, QuestionsId = 2 }
        };

        _dbContextMock.Setup(x => x.ProgramTemplate.Find(programTemplateId)).Returns(existingProgramTemplate);
        _dbContextMock.Setup(x => x.QuestionsMapping.Where(q => q.ProgramTemplateId == programTemplateId)).Returns(questionsMappings.AsQueryable());

        // Act
        var result = _controller.DeleteProgram(programTemplateId);

        // Assert
        Assert.IsType<OkResult>(result);
        _dbContextMock.Verify(x => x.Remove(existingProgramTemplate), Times.Once);
        _dbContextMock.Verify(x => x.Remove(It.IsAny<QuestionsMapping>()), Times.Exactly(questionsMappings.Count));
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void GetProgram_ShouldReturnProgramAndQuestions()
    {
        // Arrange
        var programTemplateId = 1;
        var existingProgramTemplate = new ProgramTemplate
        {
            Id = programTemplateId,
            ProgramTitle = "Sample Program",
            ProgramDescription = "Description",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        var questions = new List<Questions>
        {
            new Questions { Id = 1, Question = "What is your favorite color?", QuestionType = QuestionType.MultipleChoice, QuestionString = JsonSerializer.Serialize(new List<string> { "Red", "Blue" }) },
            new Questions { Id = 2, Question = "Describe your experience.", QuestionType = QuestionType.Text }
        };

        var questionsMappings = new List<QuestionsMapping>
        {
            new QuestionsMapping { Id = 1, ProgramTemplateId = programTemplateId, QuestionsId = 1, Questions = questions[0] },
            new QuestionsMapping { Id = 2, ProgramTemplateId = programTemplateId, QuestionsId = 2, Questions = questions[1] }
        };

        _dbContextMock.Setup(x => x.ProgramTemplate.Find(programTemplateId)).Returns(existingProgramTemplate);
        _dbContextMock.Setup(x => x.QuestionsMapping.Where(q => q.ProgramTemplateId == programTemplateId)).Returns(questionsMappings.AsQueryable());

        // Act
        var result = _controller.GetProgram(programTemplateId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ProgramAndQuestionDTO>(okResult.Value);
        Assert.Equal(programTemplateId, returnValue.ProgramTemplate.Id);
        Assert.Equal(questions.Count, returnValue.Questions.Count);
    }

    [Fact]
    public void GetAllPrograms_ShouldReturnAllPrograms()
    {
        // Arrange
        var programs = new List<ProgramTemplate>
        {
            new ProgramTemplate { Id = 1, ProgramTitle = "Program 1", ProgramDescription = "Description 1", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
            new ProgramTemplate { Id = 2, ProgramTitle = "Program 2", ProgramDescription = "Description 2", FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" }
        };

        _dbContextMock.Setup(x => x.ProgramTemplate.ToList()).Returns(programs);

        // Act
        var result = _controller.GetAllPrograms();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<ProgramTemplateDTO>>(okResult.Value);
        Assert.Equal(programs.Count, returnValue.Count);
    }
}
