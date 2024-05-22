using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LXP.Core.IServices;
using LXP.Common.DTO;
using LXP.Data.DBContexts;


namespace LXP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly LXPDbContext _LXPDbContext;

        public QuizController(IQuizService quizService, LXPDbContext dbContext)
        {
            _quizService = quizService;
            _LXPDbContext = dbContext;
        }



        [HttpGet("{id}")]
        public ActionResult<QuizDto> GetQuizById(Guid id)
        {
            var quiz = _quizService.GetQuizById(id);
            if (quiz == null)
                throw new Exception($"Quiz with id {id} not found.");

            var quizResponse = new
            {
                quiz.QuizId,
                quiz.NameOfQuiz,
                quiz.Duration,
                quiz.PassMark,
                quiz.AttemptsAllowed 
            };

            return Ok(quizResponse);
        }

        [HttpGet]
        public ActionResult<IEnumerable<QuizDto>> GetAllQuizzes()
        {
            var quizzes = _quizService.GetAllQuizzes();
            var quizResponse = quizzes.Select(quiz => new
            {
                quiz.QuizId,
                quiz.NameOfQuiz,
                quiz.Duration,
                quiz.PassMark,
                quiz.AttemptsAllowed
            });

            return Ok(quizResponse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult CreateQuiz([FromBody] CreateQuizDto request)
        {
            var cId = _LXPDbContext.Topics.Where(q => q.TopicId == request.TopicId).Select(q => q.CourseId).FirstOrDefault();


            var quizId = Guid.NewGuid(); // Generate QuizId
            var courseId = cId; // Hardcoded CourseId
            var createdBy = "System"; // Set createdBy
            var createdAt = DateTime.UtcNow; // Set createdAt

            // Validate AttemptsAllowed
            if (request.AttemptsAllowed.HasValue && request.AttemptsAllowed <= 0)
                throw new Exception("AttemptsAllowed must be null or a positive integer.");

            // Validate NameOfQuiz
            if (string.IsNullOrWhiteSpace(request.NameOfQuiz))
                throw new Exception("NameOfQuiz cannot be null or empty.");

            // Validate Duration
            if (request.Duration <= 0)
                throw new Exception("Duration must be a positive integer.");

            // Validate PassMark
            if (request.PassMark <= 0)
                throw new Exception("PassMark must be a positive integer.");
            var quiz = new QuizDto
            {
                QuizId = quizId,
                CourseId = courseId,
                TopicId = request.TopicId,
                NameOfQuiz = request.NameOfQuiz,
                Duration = request.Duration,
                PassMark = request.PassMark,
                AttemptsAllowed = request.AttemptsAllowed, // Assign AttemptsAllowed
                CreatedBy = createdBy,
                CreatedAt = createdAt
            };

            // Pass the necessary fields to the service
            _quizService.CreateQuiz(quiz);

            // Return 201 Created status with the newly created quiz
            return CreatedAtAction(nameof(GetQuizById), new { id = quizId }, new { quiz.NameOfQuiz, quiz.Duration, quiz.PassMark, quiz.AttemptsAllowed });
        }

        [HttpPut("{id}")]
        public ActionResult UpdateQuiz(Guid id, [FromBody] UpdateQuizDto request)
        {
            // Validate quiz existence
            var existingQuiz = _quizService.GetQuizById(id);
            if (existingQuiz == null)
                throw new Exception($"Quiz with id {id} not found.");

            // Validate AttemptsAllowed
            if (request.AttemptsAllowed.HasValue && request.AttemptsAllowed <= 0)
                throw new Exception("AttemptsAllowed must be null or a positive integer.");

            // Validate NameOfQuiz
            if (string.IsNullOrWhiteSpace(request.NameOfQuiz))
                throw new Exception("NameOfQuiz cannot be null or empty.");

            // Validate Duration
            if (request.Duration <= 0)
                throw new Exception("Duration must be a positive integer.");

            // Validate PassMark
            if (request.PassMark <= 0)
                throw new Exception("PassMark must be a positive integer.");

            // Update only the allowed fields
            existingQuiz.NameOfQuiz = request.NameOfQuiz;
            existingQuiz.Duration = request.Duration;
            existingQuiz.PassMark = request.PassMark;
            existingQuiz.AttemptsAllowed = request.AttemptsAllowed; // Assign AttemptsAllowed

            // Call the service method to update the quiz
            _quizService.UpdateQuiz(existingQuiz);

            // Return NoContent if successful
            return NoContent();
        }



        [HttpDelete("{id}")]
        public ActionResult DeleteQuiz(Guid id)
        {
            // Validate quiz existence
            var existingQuiz = _quizService.GetQuizById(id);
            if (existingQuiz == null)
                throw new Exception($"Quiz with id {id} not found.");

            _quizService.DeleteQuiz(id);
            return NoContent();
        }

        [HttpGet("topic/{topicId}")]
        public ActionResult<Guid?> GetQuizIdByTopicId(Guid topicId)
        {
            var quizId = _quizService.GetQuizIdByTopicId(topicId);
            if (quizId == null)
            {
                return Ok(null);
            }
            else
            {
                return Ok(quizId);
            }
            
        }
    }
}
