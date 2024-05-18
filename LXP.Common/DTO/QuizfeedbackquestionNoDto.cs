using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Common.DTO
{
    public class QuizfeedbackquestionNoDto
    {
        public Guid QuizFeedbackQuestionId { get; set; }

        public Guid QuizId { get; set; }

        public int QuestionNo {  get; set; }

        public string Question { get; set; } = null!;

        public string QuestionType { get; set; } = null!;

        public List<FeedbackquestionsoptionDto> Options { get; set; } = new List<FeedbackquestionsoptionDto>();

    }
}
