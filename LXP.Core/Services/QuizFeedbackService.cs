using LXP.Common.DTO;
using LXP.Core.IServices;
using LXP.Data.IRepository;
using System;
using System.Collections.Generic;

namespace LXP.Core.Services
{
    public class QuizFeedbackService : IQuizFeedbackService
    {
        private readonly IQuizFeedbackRepository _quizFeedbackRepository;

        public QuizFeedbackService(IQuizFeedbackRepository quizFeedbackRepository)
        {
            _quizFeedbackRepository = quizFeedbackRepository;
        }

        public Guid AddFeedbackQuestion(QuizfeedbackquestionDto quizfeedbackquestionDto, List<FeedbackquestionsoptionDto> options)
        {            
            return _quizFeedbackRepository.AddFeedbackQuestion(quizfeedbackquestionDto, options);
        }

        public List<QuizfeedbackquestionNoDto> GetAllFeedbackQuestions()
        {           
            return _quizFeedbackRepository.GetAllFeedbackQuestions();
        }

        public QuizfeedbackquestionNoDto GetFeedbackQuestionById(Guid quizFeedbackQuestionId)
        {          
            return _quizFeedbackRepository.GetFeedbackQuestionById(quizFeedbackQuestionId);
        }

        public bool UpdateFeedbackQuestion(Guid quizFeedbackQuestionId, QuizfeedbackquestionDto quizfeedbackquestionDto, List<FeedbackquestionsoptionDto> options)
        {        
            return _quizFeedbackRepository.UpdateFeedbackQuestion(quizFeedbackQuestionId, quizfeedbackquestionDto, options);
        }

        public bool DeleteFeedbackQuestion(Guid quizFeedbackQuestionId)
        {
            return _quizFeedbackRepository.DeleteFeedbackQuestion(quizFeedbackQuestionId);
        }
    }
}
