using KvizHub.Api.Dtos.Question;
using KvizHub.Api.Dtos.Quiz;
using KvizHub.Api.Dtos.Result;
using KvizHub.Api.Services.Question;
using KvizHub.Api.Services.Quizzes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KvizHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizzesController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IQuestionService _questionService;

        public QuizzesController(IQuizService quizService, IQuestionService questionService) 
        {
            _quizService = quizService;
            _questionService = questionService; 
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizzes()
        {
            var quizzes = await _quizService.GetQuizzesAsync();
            return Ok(quizzes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuizById(int id)
        {
            var quiz = await _quizService.GetQuizByIdAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }
            // TODO: Vrati odgovarajući DTO za prikaz jednog kviza, ne ceo model
            return Ok(quiz);
        }

        [HttpGet("{id}/take")]
        public async Task<ActionResult<QuizForTakerDto>> GetQuizForTaker(int id)
        {
            var quiz = await _quizService.GetQuizForTakerAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }
            return Ok(quiz);
        }

        [HttpGet("{quizId}/questions")]
        public async Task<IActionResult> GetQuestionsForQuiz(int quizId)
        {
            var quizExists = await _quizService.GetQuizByIdAsync(quizId);
            if (quizExists == null)
            {
                return NotFound($"Kviz sa ID-jem {quizId} nije pronađen.");
            }

            var questions = await _questionService.GetQuestionsForQuizAsync(quizId);
            return Ok(questions);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateQuiz(CreateQuizWithQuestionsDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdQuiz = await _quizService.CreateQuiz(dto);

            if (createdQuiz == null)
            {
                return Conflict("Kviz sa tim imenom već postoji.");
            }

            return CreatedAtAction(nameof(GetQuizById), new { id = createdQuiz.QuizID }, createdQuiz);
        }

        [HttpPost("{quizId}/questions")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddQuestionToQuiz(int quizId, CreateQuestionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newQuestionDto = await _questionService.AddQuestionToQuizAsync(quizId, dto);

            if (newQuestionDto == null)
            {
                return NotFound($"Kviz sa ID-jem {quizId} nije pronađen.");
            }

            return StatusCode(201, newQuestionDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateQuiz(int id, UpdateQuizDto updateQuizDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updatedQuiz = await _quizService.UpdateQuizAsync(id, updateQuizDto);
            if (updatedQuiz == null) return NotFound();
            return Ok(updatedQuiz);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var success = await _quizService.DeleteQuizAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

    }
}