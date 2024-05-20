using DotNetTask.DTO;
using DotNetTask.Models;
using DotNetTask.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DotNetTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProgramController : ControllerBase
    {
        private readonly DotNetTaskDbContext _dbContext;
        private readonly ILogger<ProgramController> _logger;

        public ProgramController(ILogger<ProgramController> logger, DotNetTaskDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult SaveProgram(ProgramAndQuestionDTO programAndQuestionDTO)
        {
            try
            {
                var questionsMappingList = new List<QuestionsMapping>();
                var programTemplate = new ProgramTemplate
                {
                    ProgramTitle = programAndQuestionDTO.ProgramTemplate.ProgramTitle,
                    ProgramDescription = programAndQuestionDTO.ProgramTemplate.ProgramDescription,
                    FirstName = programAndQuestionDTO.ProgramTemplate.FirstName,
                    LastName = programAndQuestionDTO.ProgramTemplate.LastName,
                    Email = programAndQuestionDTO.ProgramTemplate.Email,
                    Phone = programAndQuestionDTO.ProgramTemplate.Phone,
                    Nationality = programAndQuestionDTO.ProgramTemplate.Nationality,
                    IDCard = programAndQuestionDTO.ProgramTemplate.IDCard,
                    DateOfBirth = programAndQuestionDTO.ProgramTemplate.DateOfBirth,
                    Gender = programAndQuestionDTO.ProgramTemplate.Gender
                };
                _dbContext.Add(programTemplate);
                _dbContext.SaveChanges();

                foreach (var question in programAndQuestionDTO.Questions)
                {
                    var questions = new Questions
                    {
                        Question = question.Question,
                        QuestionType = question.QuestionType,
                        QuestionString = question.QuestionString != null ? JsonSerializer.Serialize(question.QuestionString) : null,
                    };
                    _dbContext.Add(questions);
                    _dbContext.SaveChanges();

                    var questionsMapping = new QuestionsMapping
                    {
                        QuestionsId = questions.Id,
                        ProgramTemplateId = programTemplate.Id
                    };
                    questionsMappingList.Add(questionsMapping);
                }

                _dbContext.AddRange(questionsMappingList);
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the program and questions.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult EditProgram(int id, ProgramAndQuestionDTO programAndQuestionDTO)
        {
            try
            {
                var existingProgram = _dbContext.ProgramTemplate.Find(id);
                if (existingProgram == null)
                {
                    return NotFound();
                }

                existingProgram.ProgramTitle = programAndQuestionDTO.ProgramTemplate.ProgramTitle;
                existingProgram.ProgramDescription = programAndQuestionDTO.ProgramTemplate.ProgramDescription;
                existingProgram.FirstName = programAndQuestionDTO.ProgramTemplate.FirstName;
                existingProgram.LastName = programAndQuestionDTO.ProgramTemplate.LastName;
                existingProgram.Email = programAndQuestionDTO.ProgramTemplate.Email;
                existingProgram.Phone = programAndQuestionDTO.ProgramTemplate.Phone;
                existingProgram.Nationality = programAndQuestionDTO.ProgramTemplate.Nationality;
                existingProgram.IDCard = programAndQuestionDTO.ProgramTemplate.IDCard;
                existingProgram.DateOfBirth = programAndQuestionDTO.ProgramTemplate.DateOfBirth;
                existingProgram.Gender = programAndQuestionDTO.ProgramTemplate.Gender;

                _dbContext.Update(existingProgram);
                _dbContext.SaveChanges();

                var existingQuestionsMappings = _dbContext.QuestionsMapping.Where(qm => qm.ProgramTemplateId == id).ToList();
                foreach (var mapping in existingQuestionsMappings)
                {
                    var existingQuestion = _dbContext.Questions.Find(mapping.QuestionsId);
                    if (existingQuestion != null)
                    {
                        _dbContext.Questions.Remove(existingQuestion);
                    }
                    _dbContext.QuestionsMapping.Remove(mapping);
                }
                _dbContext.SaveChanges();

                foreach (var question in programAndQuestionDTO.Questions)
                {
                    Questions questions = new Questions
                    {
                        Question = question.Question,
                        QuestionType = question.QuestionType,
                        QuestionString = question.QuestionString != null ? JsonSerializer.Serialize(question.QuestionString) : null,
                    };
                    _dbContext.Add(questions);
                    _dbContext.SaveChanges();

                    QuestionsMapping questionsMapping = new QuestionsMapping
                    {
                        QuestionsId = questions.Id,
                        ProgramTemplateId = existingProgram.Id
                    };
                    _dbContext.Add(questionsMapping);
                    _dbContext.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing the program and questions.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProgram(int id)
        {
            try
            {
                var existingProgram = _dbContext.ProgramTemplate.Find(id);
                if (existingProgram == null)
                {
                    return NotFound();
                }

                var existingQuestionsMappings = _dbContext.QuestionsMapping.Where(qm => qm.ProgramTemplateId == id).ToList();
                foreach (var mapping in existingQuestionsMappings)
                {
                    var existingQuestion = _dbContext.Questions.Find(mapping.QuestionsId);
                    if (existingQuestion != null)
                    {
                        _dbContext.Questions.Remove(existingQuestion);
                    }
                    _dbContext.QuestionsMapping.Remove(mapping);
                }

                _dbContext.ProgramTemplate.Remove(existingProgram);
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the program and questions.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        public IActionResult GetAllPrograms()
        {
            try
            {
                var programs = _dbContext.ProgramTemplate.ToList();

                var programDTOs = programs.Select(program => new ProgramAndQuestionDTO
                {
                    ProgramTemplate = new ProgramTemplateDTO
                    {
                        Id = program.Id,
                        ProgramTitle = program.ProgramTitle,
                        ProgramDescription = program.ProgramDescription,
                        FirstName = program.FirstName,
                        LastName = program.LastName,
                        Email = program.Email,
                        Phone = program.Phone,
                        Nationality = program.Nationality,
                        IDCard = program.IDCard,
                        DateOfBirth = program.DateOfBirth,
                        Gender = program.Gender
                    },
                    Questions = _dbContext.QuestionsMapping
                        .Where(qm => qm.ProgramTemplateId == program.Id)
                        .Select(qm => _dbContext.Questions.Find(qm.QuestionsId))
                        .Where(q => q != null)
                        .Select(q => new QuestionDTO
                        {
                            Id = q.Id,
                            Question = q.Question,
                            QuestionType = q.QuestionType,
                            QuestionString = q.QuestionString != null ? JsonSerializer.Deserialize<string>(q.QuestionString, new JsonSerializerOptions()) : null
                        })
                        .ToList()
                }).ToList();

                return Ok(programDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all programs.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{id}")]
        public IActionResult GetProgram(int id)
        {
            try
            {
                var existingProgram = _dbContext.ProgramTemplate.Find(id);
                if (existingProgram == null)
                {
                    return NotFound();
                }

                var questionsMappings = _dbContext.QuestionsMapping
                    .Where(qm => qm.ProgramTemplateId == id)
                    .ToList();

                var questions = questionsMappings
                    .Select(qm => _dbContext.Questions.Find(qm.QuestionsId))
                    .Where(q => q != null)
                    .ToList();

                var programAndQuestionDTO = new ProgramAndQuestionDTO
                {
                    ProgramTemplate = new ProgramTemplateDTO
                    {
                        Id = existingProgram.Id,
                        ProgramTitle = existingProgram.ProgramTitle,
                        ProgramDescription = existingProgram.ProgramDescription,
                        FirstName = existingProgram.FirstName,
                        LastName = existingProgram.LastName,
                        Email = existingProgram.Email,
                        Phone = existingProgram.Phone,
                        Nationality = existingProgram.Nationality,
                        IDCard = existingProgram.IDCard,
                        DateOfBirth = existingProgram.DateOfBirth,
                        Gender = existingProgram.Gender
                    },
                    Questions = questions.Select(q => new QuestionDTO
                    {
                        Id = q.Id,
                        Question = q.Question,
                        QuestionType = q.QuestionType,
                        QuestionString = q.QuestionString != null ? JsonSerializer.Deserialize<string>(q.QuestionString) : null
                    }).ToList()
                };

                return Ok(programAndQuestionDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the program and questions.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
