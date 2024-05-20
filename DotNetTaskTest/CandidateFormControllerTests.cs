using DotNetTask.Controllers;
using DotNetTask.DTO;
using DotNetTask.Models;
using DotNetTask.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class CandidateFormControllerTests
{
    private readonly Mock<ILogger<CandidateFormController>> _loggerMock;
    private readonly Mock<DotNetTaskDbContext> _dbContextMock;
    private readonly CandidateFormController _controller;

    public CandidateFormControllerTests()
    {
        _loggerMock = new Mock<ILogger<CandidateFormController>>();
        _dbContextMock = new Mock<DotNetTaskDbContext>();
        _controller = new CandidateFormController(_loggerMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public void SaveCandidateForm_ShouldSaveFormAndAnswers()
    {
        // Arrange
        var candidateFormDTO = new CandidateFormDTO
        {
            ProgramTemplateId = 1,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            SubmittedDate = DateTime.Now
        };

        var candidateAnswers = new List<CandidateAnswerDTO>
        {
            new CandidateAnswerDTO { QuestionId = 1, Answer = "[\"Red\", \"Blue\"]" },
            new CandidateAnswerDTO { QuestionId = 2, Answer = "I have 5 years of experience in software development." }
        };

        var candidateFormAndAnswerDTO = new CandidateFormAndAnswerDTO
        {
            CandidateForm = candidateFormDTO,
            CandidateAnswers = candidateAnswers
        };

        _dbContextMock.Setup(x => x.SaveChanges()).Returns(1);

        // Act
        var result = _controller.SaveCandidateForm(candidateFormAndAnswerDTO);

        // Assert
        Assert.IsType<OkResult>(result);
        _dbContextMock.Verify(x => x.Add(It.IsAny<CandidateForm>()), Times.Once);
        _dbContextMock.Verify(x => x.Add(It.IsAny<CandidateAnswer>()), Times.Exactly(candidateAnswers.Count));
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Exactly(2)); // 1 for form and 1 for answers
    }

    [Fact]
    public void EditCandidateForm_ShouldUpdateFormAndAnswers()
    {
        // Arrange
        var candidateFormId = 1;
        var existingCandidateForm = new CandidateForm
        {
            Id = candidateFormId,
            ProgramTemplateId = 1,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            SubmittedDate = DateTime.Now
        };

        var candidateFormDTO = new CandidateFormDTO
        {
            ProgramTemplateId = 1,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            SubmittedDate = DateTime.Now
        };

        var candidateAnswers = new List<CandidateAnswerDTO>
        {
            new CandidateAnswerDTO { QuestionId = 1, Answer = "[\"Green\", \"Yellow\"]" },
            new CandidateAnswerDTO { QuestionId = 2, Answer = "I have 6 years of experience in software development." }
        };

        var candidateFormAndAnswerDTO = new CandidateFormAndAnswerDTO
        {
            CandidateForm = candidateFormDTO,
            CandidateAnswers = candidateAnswers
        };

        var mockCandidateForms = new List<CandidateForm> { existingCandidateForm }.AsQueryable();
        var mockCandidateAnswers = new List<CandidateAnswer>().AsQueryable();

        var mockCandidateFormSet = new Mock<DbSet<CandidateForm>>();
        mockCandidateFormSet.As<IQueryable<CandidateForm>>().Setup(m => m.Provider).Returns(mockCandidateForms.Provider);
        mockCandidateFormSet.As<IQueryable<CandidateForm>>().Setup(m => m.Expression).Returns(mockCandidateForms.Expression);
        mockCandidateFormSet.As<IQueryable<CandidateForm>>().Setup(m => m.ElementType).Returns(mockCandidateForms.ElementType);
        mockCandidateFormSet.As<IQueryable<CandidateForm>>().Setup(m => m.GetEnumerator()).Returns(mockCandidateForms.GetEnumerator());

        var mockCandidateAnswerSet = new Mock<DbSet<CandidateAnswer>>();
        mockCandidateAnswerSet.As<IQueryable<CandidateAnswer>>().Setup(m => m.Provider).Returns(mockCandidateAnswers.Provider);
        mockCandidateAnswerSet.As<IQueryable<CandidateAnswer>>().Setup(m => m.Expression).Returns(mockCandidateAnswers.Expression);
        mockCandidateAnswerSet.As<IQueryable<CandidateAnswer>>().Setup(m => m.ElementType).Returns(mockCandidateAnswers.ElementType);
        mockCandidateAnswerSet.As<IQueryable<CandidateAnswer>>().Setup(m => m.GetEnumerator()).Returns(mockCandidateAnswers.GetEnumerator());

        _dbContextMock.Setup(x => x.CandidateForms).Returns(mockCandidateFormSet.Object);
        _dbContextMock.Setup(x => x.CandidateAnswers).Returns(mockCandidateAnswerSet.Object);
        _dbContextMock.Setup(x => x.CandidateForms.Find(candidateFormId)).Returns(existingCandidateForm);
        _dbContextMock.Setup(x => x.SaveChanges()).Returns(1);

        // Act
        var result = _controller.EditCandidateForm(candidateFormId, candidateFormAndAnswerDTO);

        // Assert
        Assert.IsType<OkResult>(result);
        _dbContextMock.Verify(x => x.Update(existingCandidateForm), Times.Once);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Exactly(3)); // 1 for form and 2 for answers
    }

    [Fact]
    public void DeleteCandidateForm_ShouldRemoveFormAndAnswers()
    {
        // Arrange
        var candidateFormId = 1;
        var existingCandidateForm = new CandidateForm
        {
            Id = candidateFormId,
            ProgramTemplateId = 1,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            SubmittedDate = DateTime.Now
        };

        var candidateAnswers = new List<CandidateAnswer>
        {
            new CandidateAnswer { CandidateFormId = candidateFormId, QuestionId = 1, Answer = "[\"Red\", \"Blue\"]" },
            new CandidateAnswer { CandidateFormId = candidateFormId, QuestionId = 2, Answer = "I have 5 years of experience in software development." }
        };

        var mockCandidateForms = new List<CandidateForm> { existingCandidateForm }.AsQueryable();
        var mockCandidateAnswers = candidateAnswers.AsQueryable();

        var mockCandidateFormSet = new Mock<DbSet<CandidateForm>>();
        mockCandidateFormSet.As<IQueryable<CandidateForm>>().Setup(m => m.Provider).Returns(mockCandidateForms.Provider);
        mockCandidateFormSet.As<IQueryable<CandidateForm>>().Setup(m => m.Expression).Returns(mockCandidateForms.Expression);
        mockCandidateFormSet.As<IQueryable<CandidateForm>>().Setup(m => m.ElementType).Returns(mockCandidateForms.ElementType);
        mockCandidateFormSet.As<IQueryable<CandidateForm>>().Setup(m => m.GetEnumerator()).Returns(mockCandidateForms.GetEnumerator());

        var mockCandidateAnswerSet = new Mock<DbSet<CandidateAnswer>>();
        mockCandidateAnswerSet.As<IQueryable<CandidateAnswer>>().Setup(m => m.Provider).Returns(mockCandidateAnswers.Provider);
        mockCandidateAnswerSet.As<IQueryable<CandidateAnswer>>().Setup(m => m.Expression).Returns(mockCandidateAnswers.Expression);
        mockCandidateAnswerSet.As<IQueryable<CandidateAnswer>>().Setup(m => m.ElementType).Returns(mockCandidateAnswers.ElementType);
        mockCandidateAnswerSet.As<IQueryable<CandidateAnswer>>().Setup(m => m.GetEnumerator()).Returns(mockCandidateAnswers.GetEnumerator());

        _dbContextMock.Setup(x => x.CandidateForms).Returns(mockCandidateFormSet.Object);
        _dbContextMock.Setup(x => x.CandidateAnswers).Returns(mockCandidateAnswerSet.Object);
        _dbContextMock.Setup(x => x.CandidateForms.Find(candidateFormId)).Returns(existingCandidateForm);
        _dbContextMock.Setup(x => x.SaveChanges()).Returns(1);

        // Act
        var result = _controller.DeleteCandidateForm(candidateFormId);

        // Assert
        Assert.IsType<OkResult>(result);
        _dbContextMock.Verify(x => x.Remove(existingCandidateForm), Times.Once);
        _dbContextMock.Verify(x => x.Remove(It.IsAny<CandidateAnswer>()), Times.Exactly(candidateAnswers.Count));
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void GetCandidateForm_ReturnsOkResult_WithCandidateFormDTO()
    {
        // Arrange
        var candidateForms = new List<CandidateForm>
        {
            new CandidateForm { Id = 1, ProgramTemplateId = 1, FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", SubmittedDate = DateTime.Now },
            new CandidateForm { Id = 2, ProgramTemplateId = 2, FirstName = "John", LastName = "Smith", Email = "john.smith@example.com", SubmittedDate = DateTime.Now }
}.AsQueryable();
        var mockSet = new Mock<DbSet<CandidateForm>>();
        mockSet.As<IQueryable<CandidateForm>>().Setup(m => m.Provider).Returns(candidateForms.Provider);
        mockSet.As<IQueryable<CandidateForm>>().Setup(m => m.Expression).Returns(candidateForms.Expression);
        mockSet.As<IQueryable<CandidateForm>>().Setup(m => m.ElementType).Returns(candidateForms.ElementType);
        mockSet.As<IQueryable<CandidateForm>>().Setup(m => m.GetEnumerator()).Returns(candidateForms.GetEnumerator());

        _dbContextMock.Setup(x => x.CandidateForms).Returns(mockSet.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Returns(1);

        // Act
        var result = _controller.GetCandidateForm(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<CandidateFormAndAnswerDTO>(okResult.Value);
        Assert.Equal(1, returnValue.CandidateForm.Id);
    }

    [Fact]
    public void GetAllCandidateForms_ShouldReturnAllForms()
    {
        // Arrange
        var candidateForms = new List<CandidateForm>
    {
        new CandidateForm { Id = 1, ProgramTemplateId = 1, FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", SubmittedDate = DateTime.Now },
        new CandidateForm { Id = 2, ProgramTemplateId = 2, FirstName = "John", LastName = "Smith", Email = "john.smith@example.com", SubmittedDate = DateTime.Now }
    }.AsQueryable();

        var mockSet = new Mock<DbSet<CandidateForm>>();
        mockSet.As<IQueryable<CandidateForm>>().Setup(m => m.Provider).Returns(candidateForms.Provider);
        mockSet.As<IQueryable<CandidateForm>>().Setup(m => m.Expression).Returns(candidateForms.Expression);
        mockSet.As<IQueryable<CandidateForm>>().Setup(m => m.ElementType).Returns(candidateForms.ElementType);
        mockSet.As<IQueryable<CandidateForm>>().Setup(m => m.GetEnumerator()).Returns(candidateForms.GetEnumerator());

        _dbContextMock.Setup(x => x.CandidateForms).Returns(mockSet.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Returns(1);

        // Act
        var result = _controller.GetAllCandidateForms();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<CandidateFormDTO>>(okResult.Value);
        Assert.Equal(candidateForms.Count(), returnValue.Count);
    }
}
