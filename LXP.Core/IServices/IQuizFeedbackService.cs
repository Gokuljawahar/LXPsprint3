using LXP.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Core.IServices
{
    public interface IQuizFeedbackService
    {
        Guid AddFeedbackQuestion(QuizfeedbackquestionDto quizfeedbackquestionDto, List<FeedbackquestionsoptionDto> options);
       
        List<QuizfeedbackquestionNoDto> GetAllFeedbackQuestions();

        QuizfeedbackquestionNoDto GetFeedbackQuestionById(Guid QuizFeedbackQuestionId);
    
        bool UpdateFeedbackQuestion(Guid quizFeedbackQuestionId, QuizfeedbackquestionDto quizfeedbackquestionDto, List<FeedbackquestionsoptionDto> options);
    
        bool DeleteFeedbackQuestion(Guid quizFeedbackQuestionId);
    }
}
