using AutoMapper;
using LXP.Common.DTO;
using LXP.Core.IServices;
using LXP.Data.IRepository;

namespace LXP.Core.Services
{
    public class TopicFeedbackService : ITopicFeedbackService
    {
        private readonly ITopicFeedbackRepository _repository;

        public TopicFeedbackService(ITopicFeedbackRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<FeedbackQuestionDTO> GetAllFeedbackQuestions()
        {
            return _repository.GetAllFeedbackQuestions();
        }

        public FeedbackQuestionDTO GetFeedbackQuestionById(Guid id)
        {
            return _repository.GetFeedbackQuestionById(id);
        }

        public void SubmitFeedbackResponse(FeedbackResponseDTO feedbackResponse)
        {
            _repository.AddFeedbackResponse(feedbackResponse);
        }

        public bool AddFeedbackQuestion(FeedbackQuestionDTO question)
        {
            // Validation and business logic here
            // Example:
            if (question == null)
                return false;

            if (string.IsNullOrEmpty(question.Question))
                return false;

            //if (question.QuestionType == "MCQ" && (question.Options == null || question.Options.Count != 4))
            //    return false;

            // Map DTO to entity and save to database
            return _repository.AddFeedbackQuestion(question);
        }

        // Implement other IService methods
    }
}
