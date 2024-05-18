using LXP.Common.DTO;
using LXP.Data.DBContexts;
using LXP.Data.IRepository;

namespace LXP.Data.Repository
{
    public class TopicFeedbackRepository : ITopicFeedbackRepository
    {
        private readonly LXPDbContext _context;

        public TopicFeedbackRepository(LXPDbContext context)
        {
            _context = context;
        }

        public bool AddFeedbackQuestion(FeedbackQuestionDTO question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));



            int questionCount = _context.Topicfeedbackquestions.Count(q => q.TopicId == question.TopicId);

            int questionNo = questionCount + 1;

            var feedbackQuestion = new Topicfeedbackquestion
            {
                TopicId = Guid.Parse("e3a895e4-1b3f-45b8-9c0a-98f9c0fa4996"),
                QuestionNo = questionNo,
                Question = question.Question,
                QuestionType = question.QuestionType
            };

            if (question.QuestionType == "MCQ" && question.Options != null)
            {
                foreach (var optionText in question.Options)
                {
                    feedbackQuestion.Feedbackquestionsoptions.Add(new Feedbackquestionsoption
                    {
                        OptionText = optionText
                    });
                }
            }

            _context.Topicfeedbackquestions.Add(feedbackQuestion);
            _context.SaveChanges();

            return true;
        }

        public IEnumerable<FeedbackQuestionDTO> GetAllFeedbackQuestions()
        {
            return _context.Topicfeedbackquestions
                .Select(q => new FeedbackQuestionDTO
                {
                    Id = q.TopicFeedbackQuestionId,
                    TopicId = q.TopicId,
                    QuestionNo = q.QuestionNo,
                    Question = q.Question,
                    QuestionType = q.QuestionType
                    // Map other properties as needed
                })
                .ToList();
        }

        public FeedbackQuestionDTO GetFeedbackQuestionById(Guid id)
        {
            var question = _context.Topicfeedbackquestions.FirstOrDefault(q => q.TopicFeedbackQuestionId == id);
            if (question == null)
                return null;

            return new FeedbackQuestionDTO
            {
                Id = question.TopicFeedbackQuestionId,
                Question = question.Question,
                QuestionType = question.QuestionType
                // Map other properties as needed
            };
        }

        public void AddFeedbackResponse(FeedbackResponseDTO feedbackResponse)
        {
            var response = new Feedbackresponse
            {
                TopicFeedbackQuestionId = feedbackResponse.TopicFeedbackQuestionId,
                LearnerId = feedbackResponse.LearnerId,
                Response = feedbackResponse.Response,
                OptionId = feedbackResponse.OptionId,
                // Map other properties as needed
            };

            _context.Feedbackresponses.Add(response);
            _context.SaveChanges();
        }

        // Implement other IRepository methods
    }
}
