using System;
using System.Collections.Generic;
using LXP.Common.DTO;

namespace LXP.Data.IRepository
{
    public interface IQuizFeedbackRepository
    {
        Guid AddFeedbackQuestion(QuizFeedbackQuestionDto quizfeedbackquestionDto, List<QuizFeedbackQuestionsOptionDto> options);
        List<QuizFeedbackQuestionNoDto> GetAllFeedbackQuestions();
        //void DecrementFeedbackQuestionNos(Guid deletedQuestionId);
        int GetNextFeedbackQuestionNo(Guid quizId);
        Guid AddFeedbackQuestionOption(QuizFeedbackQuestionsOptionDto feedbackquestionsoptionDto, Guid QuizFeedbackQuestionId);
        List<QuizFeedbackQuestionsOptionDto> GetFeedbackQuestionOptionsById(Guid QuizFeedbackQuestionId);
        QuizFeedbackQuestionNoDto GetFeedbackQuestionById(Guid QuizFeedbackQuestionId);
        bool ValidateOptionsByFeedbackQuestionType(string questionType, List<QuizFeedbackQuestionsOptionDto> options);
        bool UpdateFeedbackQuestion(Guid QuizFeedbackQuestionId, QuizFeedbackQuestionDto quizfeedbackquestionDto, List<QuizFeedbackQuestionsOptionDto> options);
        bool DeleteFeedbackQuestion(Guid QuizFeedbackQuestionId);
        //Guid AddFeedbackResponse(FeedbackResponseDto feedbackResponseDTO);
    }
}














































//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using LXP.Common.DTO;

//namespace LXP.Data.IRepository
//{
//    public interface IQuizFeedbackRepository
//    {
//        Guid AddFeedbackQuestion(QuizfeedbackquestionDto quizfeedbackquestionDto, List<FeedbackquestionsoptionDto> options);

//        List<QuizfeedbackquestionNoDto> GetAllFeedbackQuestions();

//        void DecrementFeedbackQuestionNos(Guid deletedQuestionId);

//        int GetNextFeedbackQuestionNo(Guid quizId);

//        Guid AddFeedbackQuestionOption(FeedbackquestionsoptionDto feedbackquestionsoptionDto, Guid QuizFeedbackQuestionId);

//        List<FeedbackquestionsoptionDto> GetFeedbackQuestionOptionsById(Guid QuizFeedbackQuestionId);

//        QuizfeedbackquestionNoDto GetFeedbackQuestionById(Guid QuizFeedbackQuestionId);

//        bool ValidateOptionsByFeedbackQuestionType(string questionType, List<FeedbackquestionsoptionDto> options);

//        bool UpdateFeedbackQuestion(Guid QuizFeedbackQuestionId, QuizfeedbackquestionDto quizfeedbackquestionDto, List<FeedbackquestionsoptionDto> options);

//        bool DeleteFeedbackQuestion(Guid QuizFeedbackQuestionId);


//    }
//}
