﻿
using LXP.Common.DTO;
using LXP.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LXP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizQuestionsController : ControllerBase
    {
        private readonly IQuizQuestionService _quizQuestionService;

        public QuizQuestionsController(IQuizQuestionService quizQuestionService)
        {
            _quizQuestionService = quizQuestionService;
        }

        [HttpPost("AddQuestion")]
        public IActionResult AddQuestion([FromBody] QuizQuestionDto quizQuestionDto)
        {
            var result = _quizQuestionService.AddQuestion(quizQuestionDto, quizQuestionDto.Options);
            return Ok(result);
        }        
        [HttpPut("UpdateQuestion")]
        public IActionResult UpdateQuestion(Guid quizQuestionId, [FromBody] QuizQuestionDto quizQuestionDto)
        {
            var result = _quizQuestionService.UpdateQuestion(quizQuestionId, quizQuestionDto, quizQuestionDto.Options);
            return Ok(result);
        }

        [HttpDelete("DeleteQuestion")]
        public IActionResult DeleteQuestion(Guid quizQuestionId)
        {
            var result = _quizQuestionService.DeleteQuestion(quizQuestionId);
            return Ok(result);
        }

        [HttpGet("GetAllQuestions")]
        public IActionResult GetAllQuestions()
        {
            var result = _quizQuestionService.GetAllQuestions();
            return Ok(result);
        }

        [HttpGet("GetQuestionById")]
        public IActionResult GetQuestionById(Guid quizQuestionId)
        {
            var result = _quizQuestionService.GetQuestionById(quizQuestionId);
            if (result == null)
            {
                return NotFound(); // Return 404 if question with given ID is not found
            }
            return Ok(result);
        }
    }
}
////////using Microsoft.AspNetCore.Http;
////////using Microsoft.AspNetCore.Mvc;

////////namespace LXP.Api.Controllers
////////{
////////    [Route("api/[controller]")]
////////    [ApiController]
////////    public class QuizQuestionController : ControllerBase
////////    {
////////    }
////////}

//////using System;
//////using System.Collections.Generic;
//////using System.Linq;
//////using System.Threading.Tasks;
//////using LXP.Core.IServices;
//////using Microsoft.AspNetCore.Http;
//////using Microsoft.AspNetCore.Mvc;

//////namespace LXP.Api.Controllers
//////{
//////    [Route("api/[controller]")]
//////    [ApiController]
//////    public class QuizQuestionController : ControllerBase
//////    {
//////        private readonly IQuizQuestionService _quizQuestionService;

//////        public QuizQuestionController(IQuizQuestionService quizQuestionService)
//////        {
//////            _quizQuestionService = quizQuestionService;
//////        }

//////        /// <summary>
//////        /// Adds a new quiz question to the database.
//////        /// </summary>
//////        /// <param name="question">The quiz question details.</param>
//////        /// <returns>The newly created quiz question information.</returns>
//////        [HttpPost]
//////        public async Task<ActionResult<QuizQuestionDto>> AddQuestion(QuizQuestionDto question)
//////        {
//////            if (!ModelState.IsValid)
//////            {
//////                return BadRequest(ModelState);
//////            }

//////            try
//////            {
//////                var createdQuestion = await _quizQuestionService.AddQuestionAsync(question);
//////                return CreatedAtRoute("GetQuestion", new { id = createdQuestion.QuizQuestionId }, createdQuestion);
//////            }
//////            catch (Exception ex)
//////            {
//////                return StatusCode(500, ex.Message); // Handle exceptions appropriately
//////            }
//////        }

//////        /// <summary>
//////        /// Deletes a quiz question by its ID.
//////        /// </summary>
//////        /// <param name="id">The ID of the quiz question to delete.</param>
//////        /// <returns></returns>
//////        [HttpDelete("{id}")]
//////        public async Task<IActionResult> DeleteQuestion(Guid id)
//////        {
//////            if (id == Guid.Empty)
//////            {
//////                return BadRequest("Invalid quiz question ID.");
//////            }

//////            try
//////            {
//////                await _quizQuestionService.DeleteQuestionAsync(id);
//////                return NoContent();
//////            }
//////            catch (Exception ex)
//////            {
//////                return StatusCode(500, ex.Message); // Handle exceptions appropriately
//////            }
//////        }

//////        /// <summary>
//////        /// Updates an existing quiz question in the database.
//////        /// </summary>
//////        /// <param name="id">The ID of the quiz question to update.</param>
//////        /// <param name="question">The updated quiz question details.</param>
//////        /// <returns></returns>
//////        [HttpPut("{id}")]
//////        public async Task<IActionResult> UpdateQuestion(Guid id, QuizQuestionDto question)
//////        {
//////            if (id == Guid.Empty || id != question.QuizQuestionId)
//////            {
//////                return BadRequest("Invalid quiz question ID or mismatch between ID and question object.");
//////            }

//////            if (!ModelState.IsValid)
//////            {
//////                return BadRequest(ModelState);
//////            }

//////            try
//////            {
//////                await _quizQuestionService.UpdateQuestionAsync(question);
//////                return NoContent();
//////            }
//////            catch (Exception ex)
//////            {
//////                return StatusCode(500, ex.Message); // Handle exceptions appropriately
//////            }
//////        }

//////        /// <summary>
//////        /// Gets all quiz questions from the database.
//////        /// </summary>
//////        /// <returns>A list of all quiz questions.</returns>
//////        [HttpGet]
//////        public async Task<ActionResult<IEnumerable<QuizQuestionDto>>> GetQuestions()
//////        {
//////            try
//////            {
//////                var questions = await _quizQuestionService.GetQuestionsAsync();
//////                return Ok(questions);
//////            }
//////            catch (Exception ex)
//////            {
//////                return StatusCode(500, ex.Message); // Handle exceptions appropriately
//////            }
//////        }
//////    }
//////}
////using LXP.Common.DTO;
////using LXP.Core.IServices;
////using Microsoft.AspNetCore.Http;
////using Microsoft.AspNetCore.Mvc;

////namespace LXP.Api.Controllers
////{
////    [Route("api/[controller]")]
////    [ApiController]
////    public class QuizQuestionsController : ControllerBase
////    {
////        private readonly IQuizQuestionService _quizQuestionService;

////        public QuizQuestionsController(IQuizQuestionService quizQuestionService)
////        {
////            _quizQuestionService = quizQuestionService;
////        }

////        [HttpPost("AddQuestion")]
////        public IActionResult AddQuestion(QuizQuestionDto quizQuestionDto)
////        {
////            var result = _quizQuestionService.AddQuestion(quizQuestionDto);
////            return Ok(result);
////        }

////        [HttpPut("UpdateQuestion")]
////        public IActionResult UpdateQuestion(QuizQuestionDto quizQuestionDto)
////        {
////            var result = _quizQuestionService.UpdateQuestion(quizQuestionDto);
////            return Ok(result);
////        }

////        [HttpDelete("DeleteQuestion")]
////        public IActionResult DeleteQuestion(Guid quizQuestionId)
////        {
////            var result = _quizQuestionService.DeleteQuestion(quizQuestionId);
////            return Ok(result);
////        }

////        [HttpGet("GetAllQuestions")]
////        public IActionResult GetAllQuestions()
////        {
////            var result = _quizQuestionService.GetAllQuestions();
////            return Ok(result);
////        }

////        [HttpPost("AddOption")]
////        public IActionResult AddOption(QuestionOptionDto questionOptionDto)
////        {
////            var result = _quizQuestionService.AddOption(questionOptionDto);
////            return Ok(result);
////        }
////    }
////}
//using LXP.Common.DTO;
//using LXP.Core.IServices;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace LXP.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class QuizQuestionsController : ControllerBase
//    {
//        private readonly IQuizQuestionService _quizQuestionService;

//        public QuizQuestionsController(IQuizQuestionService quizQuestionService)
//        {
//            _quizQuestionService = quizQuestionService;
//        }

//        [HttpPost("AddQuestion")]
//        public IActionResult AddQuestion(QuizQuestionDto quizQuestionDto)
//        {
//            var result = _quizQuestionService.AddQuestion(quizQuestionDto);
//            return Ok(result);
//        }

//        [HttpPut("UpdateQuestion")]
//        public IActionResult UpdateQuestion(QuizQuestionDto quizQuestionDto)
//        {
//            var result = _quizQuestionService.UpdateQuestion(quizQuestionDto);
//            return Ok(result);
//        }

//        [HttpDelete("DeleteQuestion")]
//        public IActionResult DeleteQuestion(Guid quizQuestionId)
//        {
//            var result = _quizQuestionService.DeleteQuestion(quizQuestionId);
//            return Ok(result);
//        }

//        [HttpGet("GetAllQuestions")]
//        public IActionResult GetAllQuestions()
//        {
//            var result = _quizQuestionService.GetAllQuestions();
//            return Ok(result);
//        }

//        [HttpPost("AddOption")]
//        public IActionResult AddOption(QuestionOptionDto questionOptionDto)
//        {
//            var result = _quizQuestionService.AddOption(questionOptionDto);
//            if (result != Guid.Empty)
//            {
//                return Ok(result);
//            }
//            return BadRequest("Invalid options for the question type.");
//        }
//    }
//}
//using LXP.Common.DTO;
//using LXP.Core.IServices;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace LXP.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class QuizQuestionsController : ControllerBase
//    {
//        private readonly IQuizQuestionService _quizQuestionService;

//        public QuizQuestionsController(IQuizQuestionService quizQuestionService)
//        {
//            _quizQuestionService = quizQuestionService;
//        }

//        [HttpPost("AddQuestion")]
//        public IActionResult AddQuestion(QuizQuestionDto quizQuestionDto)
//        {
//            var result = _quizQuestionService.AddQuestion(quizQuestionDto);
//            return Ok(result);
//        }

//        [HttpPut("UpdateQuestion")]
//        public IActionResult UpdateQuestion(QuizQuestionDto quizQuestionDto)
//        {
//            var result = _quizQuestionService.UpdateQuestion(quizQuestionDto);
//            return Ok(result);
//        }

//        [HttpDelete("DeleteQuestion")]
//        public IActionResult DeleteQuestion(Guid quizQuestionId)
//        {
//            var result = _quizQuestionService.DeleteQuestion(quizQuestionId);
//            return Ok(result);
//        }

//        [HttpGet("GetAllQuestions")]
//        public IActionResult GetAllQuestions()
//        {
//            var result = _quizQuestionService.GetAllQuestions();
//            return Ok(result);
//        }

//        [HttpPost("AddOption")]
//        public IActionResult AddOption(QuestionOptionDto questionOptionDto)
//        {
//            var result = _quizQuestionService.AddOption(questionOptionDto);
//            if (result != Guid.Empty)
//            {
//                return Ok(result);
//            }
//            return BadRequest("Invalid options for the question type.");
//        }
//    }
//}
//[HttpPost("AddQuestion")]

//public IActionResult AddQuestion([FromBody] QuizQuestionDto quizQuestionDto, [FromQuery] List<QuestionOptionDto> options)

//{

//    var result = _quizQuestionService.AddQuestion(quizQuestionDto, options);
//    return Ok(result);
//}
//[HttpPut("UpdateQuestion")]
//public IActionResult UpdateQuestion(Guid quizQuestionId, [FromBody] QuizQuestionDto quizQuestionDto, [FromQuery] List<QuestionOptionDto> options)
//{
//    var result = _quizQuestionService.UpdateQuestion(quizQuestionId, quizQuestionDto, options);
//    return Ok(result);
//}
