using DotNetTask.DTO;
using DotNetTask.Models;
using DotNetTask.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DotNetTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CandidateFormController : ControllerBase
    {
        private readonly DotNetTaskDbContext _dbContext;
        private readonly ILogger<CandidateFormController> _logger;

        public CandidateFormController(ILogger<CandidateFormController> logger, DotNetTaskDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult SaveCandidateForm(CandidateFormAndAnswerDTO candidateFormAndAnswerDTO)
        {
            try
            {
                var candidateForm = new CandidateForm
                {
                    ProgramTemplateId = candidateFormAndAnswerDTO.CandidateForm.ProgramTemplateId,
                    FirstName = candidateFormAndAnswerDTO.CandidateForm.FirstName,
                    LastName = candidateFormAndAnswerDTO.CandidateForm.LastName,
                    Email = candidateFormAndAnswerDTO.CandidateForm.Email,
                    SubmittedDate = candidateFormAndAnswerDTO.CandidateForm.SubmittedDate
                };

                _dbContext.Add(candidateForm);
                _dbContext.SaveChanges();

                foreach (var answer in candidateFormAndAnswerDTO.CandidateAnswers)
                {
                    var candidateAnswer = new CandidateAnswer
                    {
                        CandidateFormId = candidateForm.Id,
                        QuestionId = answer.QuestionId,
                        Answer = answer.Answer
                    };

                    _dbContext.Add(candidateAnswer);
                }

                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the candidate form and answers.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult EditCandidateForm(int id, CandidateFormAndAnswerDTO candidateFormAndAnswerDTO)
        {
            try
            {
                var existingForm = _dbContext.CandidateForms.Find(id);
                if (existingForm == null)
                {
                    return NotFound();
                }

                existingForm.FirstName = candidateFormAndAnswerDTO.CandidateForm.FirstName;
                existingForm.LastName = candidateFormAndAnswerDTO.CandidateForm.LastName;
                existingForm.Email = candidateFormAndAnswerDTO.CandidateForm.Email;
                existingForm.SubmittedDate = candidateFormAndAnswerDTO.CandidateForm.SubmittedDate;

                _dbContext.Update(existingForm);
                _dbContext.SaveChanges();

                var existingAnswers = _dbContext.CandidateAnswers.Where(ca => ca.CandidateFormId == id).ToList();
                foreach (var answer in existingAnswers)
                {
                    _dbContext.CandidateAnswers.Remove(answer);
                }

                _dbContext.SaveChanges();

                foreach (var answer in candidateFormAndAnswerDTO.CandidateAnswers)
                {
                    var candidateAnswer = new CandidateAnswer
                    {
                        CandidateFormId = existingForm.Id,
                        QuestionId = answer.QuestionId,
                        Answer = answer.Answer
                    };

                    _dbContext.Add(candidateAnswer);
                }

                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing the candidate form and answers.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCandidateForm(int id)
        {
            try
            {
                var existingForm = _dbContext.CandidateForms.Find(id);
                if (existingForm == null)
                {
                    return NotFound();
                }

                var existingAnswers = _dbContext.CandidateAnswers.Where(ca => ca.CandidateFormId == id).ToList();
                foreach (var answer in existingAnswers)
                {
                    _dbContext.CandidateAnswers.Remove(answer);
                }

                _dbContext.CandidateForms.Remove(existingForm);
                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the candidate form and answers.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetCandidateForm(int id)
        {
            try
            {
                var existingForm = _dbContext.CandidateForms.Find(id);
                if (existingForm == null)
                {
                    return NotFound();
                }

                var answers = _dbContext.CandidateAnswers.Where(ca => ca.CandidateFormId == id).ToList();

                var candidateFormAndAnswerDTO = new CandidateFormAndAnswerDTO
                {
                    CandidateForm = new CandidateFormDTO
                    {
                        Id = existingForm.Id,
                        ProgramTemplateId = existingForm.ProgramTemplateId,
                        FirstName = existingForm.FirstName,
                        LastName = existingForm.LastName,
                        Email = existingForm.Email,
                        SubmittedDate = existingForm.SubmittedDate
                    },
                    CandidateAnswers = answers.Select(a => new CandidateAnswerDTO
                    {
                        Id = a.Id,
                        CandidateFormId = a.CandidateFormId,
                        QuestionId = a.QuestionId,
                        Answer = a.Answer
                    }).ToList()
                };

                return Ok(candidateFormAndAnswerDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the candidate form and answers.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult GetAllCandidateForms()
        {
            try
            {
                var forms = _dbContext.CandidateForms.ToList();
                var candidateFormDTOs = forms.Select(f => new CandidateFormDTO
                {
                    Id = f.Id,
                    ProgramTemplateId = f.ProgramTemplateId,
                    FirstName = f.FirstName,
                    LastName = f.LastName,
                    Email = f.Email,
                    SubmittedDate = f.SubmittedDate
                }).ToList();

                return Ok(candidateFormDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all candidate forms.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}