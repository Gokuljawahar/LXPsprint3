using LXP.Common.DTO;
using LXP.Core.IServices;
using LXP.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LXP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicFeedbackController : ControllerBase
    {
        private readonly ITopicFeedbackService _service;

        public TopicFeedbackController(ITopicFeedbackService service)
        {
            _service = service;
        }


        [HttpPost("question")]
        public IActionResult AddFeedbackQuestion(FeedbackQuestionDTO question)
        {
            if (question == null)
                return BadRequest("Question object is null");

            var result = _service.AddFeedbackQuestion(question);

            if (result)
                return Ok("Question added successfully");

            return BadRequest("Failed to add question");
        }

        [HttpGet]
        public IActionResult GetAllFeedbackQuestions()
        {
            var questions = _service.GetAllFeedbackQuestions();
            return Ok(questions);
        }

        [HttpGet("{id}")]
        public IActionResult GetFeedbackQuestionById(Guid id)
        {
            var question = _service.GetFeedbackQuestionById(id);
            if (question == null)
                return NotFound();

            return Ok(question);
        }

        [HttpPost("response")]
        public IActionResult SubmitFeedbackResponse(FeedbackResponseDTO feedbackResponse)
        {
            _service.SubmitFeedbackResponse(feedbackResponse);
            return Ok();
        }

        // Implement other controller actions as needed
    }
}
